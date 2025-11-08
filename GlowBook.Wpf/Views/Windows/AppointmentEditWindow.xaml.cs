using GlowBook.Model.Data;
using GlowBook.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Globalization;
using System.Windows;

namespace GlowBook.Wpf.Views.Windows
{
    /// <summary>
    /// Interaction logic for AppointmentEditWindow.xaml
    /// </summary>
    public partial class AppointmentEditWindow : Window
    {
        private readonly int? _id;
        private readonly AppDbContext _db;
        private Appointment _appointment = null!;

        public AppointmentEditWindow(int? appointmentId)
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;

            _id = appointmentId;
            _db = App.Services.GetRequiredService<AppDbContext>();

            Loaded += (_, __) => LoadData();
        }

        private void LoadData()
        {
            // basislijsten 
            CmbCustomer.ItemsSource = _db.Customers.Where(c => !c.IsDeleted).OrderBy(c => c.Name).ToList();
            CmbStaff.ItemsSource = _db.Staff.Where(s => !s.IsDeleted).OrderBy(s => s.Name).ToList();
            CmbService.ItemsSource = _db.Services.Where(s => !s.IsDeleted).OrderBy(s => s.Name).ToList();

            if (_id == null)
            {

                _appointment = new Appointment { };
                return;
            }

            // afspraak plus eerste dienst
            _appointment = _db.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Staff)
                .Include(a => a.AppointmentServices).ThenInclude(x => x.Service)
                .FirstOrDefault(a => a.Id == _id)!;

            if (_appointment == null)
            {
                MessageBox.Show("Afspraak niet gevonden.", "GlowBook", MessageBoxButton.OK, MessageBoxImage.Warning);
                DialogResult = false;
                Close();
                return;
            }

            CmbCustomer.SelectedValue = _appointment.CustomerId;
            CmbStaff.SelectedValue = _appointment.StaffId;

            var firstSvc = _appointment.AppointmentServices.FirstOrDefault();
            if (firstSvc != null)
            {
                CmbService.SelectedValue = firstSvc.ServiceId;
                TxtDuration.Text = firstSvc.Service.DurationMin.ToString(CultureInfo.InvariantCulture);
            }

            DpDate.SelectedDate = _appointment.Start.Date;
            TxtStart.Text = _appointment.Start.ToString("HH\\:mm", CultureInfo.InvariantCulture);
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DpDate.SelectedDate == null)
                {
                    MessageBox.Show("Kies een datum.");
                    return;
                }

                // Strikt 24u-formaat HH:MM (bv. 09:30)
                if (!TimeSpan.TryParseExact(TxtStart.Text, "hh\\:mm",
                    CultureInfo.InvariantCulture, out var startTime))
                {
                    MessageBox.Show("Ongeldige starttijd (gebruik HH:MM, bv. 09:30).");
                    return;
                }

                if (!int.TryParse(TxtDuration.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var durMin))
                    durMin = 60;

                var date = DpDate.SelectedDate.Value;
                var start = date.Date + startTime;
                var end = start.AddMinutes(durMin);

                var staffId = (int?)CmbStaff.SelectedValue;
                var custId = (int?)CmbCustomer.SelectedValue;
                var svcId = (int?)CmbService.SelectedValue;

                if (staffId == null || custId == null || svcId == null)
                {
                    MessageBox.Show("Klant, medewerker en dienst zijn verplicht.");
                    return;
                }

                // Openingstijden
                var open = TimeSpan.FromHours(9);
                var close = TimeSpan.FromHours(18);
                if (start.TimeOfDay < open || end.TimeOfDay > close)
                {
                    MessageBox.Show("Buiten openingstijd (09:00–18:00).");
                    return;
                }

                // Minimale duur + buffer
                var buffer = TimeSpan.FromMinutes(5);
                var minimaleDuur = TimeSpan.FromMinutes(10) + buffer;
                if (end - start < minimaleDuur)
                {
                    MessageBox.Show($"Duur te kort (minimaal {(int)minimaleDuur.TotalMinutes} minuten incl. buffer).");
                    return;
                }

                // Overlap 
                bool conflict = await _db.Appointments
                    .Where(a => a.StaffId == staffId.Value && a.Status != "Cancelled" && a.Id != _appointment.Id)
                    .AnyAsync(a => start < a.End && end > a.Start);

                if (conflict)
                {
                    MessageBox.Show("Conflicterende afspraak voor deze medewerker in dit tijdslot.");
                    return;
                }

                // update afspraak plus eerste dienst
                _appointment.CustomerId = custId.Value;
                _appointment.StaffId = staffId.Value;
                _appointment.Start = start;
                _appointment.End = end;

                if (_id == null)
                {
                    _db.Appointments.Add(_appointment).State = EntityState.Added;
                }
                else
                {
                    _db.Appointments.Add(_appointment).State = EntityState.Modified;
                }
                    
                _db.SaveChanges();

                var link = await _db.AppointmentServices
                              .FirstOrDefaultAsync(x => x.AppointmentId == _appointment.Id);
                if (link == null)
                {
                    link = new AppointmentService
                    {
                        AppointmentId = _appointment.Id,
                        ServiceId = svcId.Value,
                        Qty = 1
                    };
                    _db.AppointmentServices.Add(link);
                }
                else
                {
                    link.ServiceId = svcId.Value;
                }

                await _db.SaveChangesAsync();
                DialogResult = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessageBox.Show("Bewaren mislukt: " + ex.Message, "GlowBook",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
