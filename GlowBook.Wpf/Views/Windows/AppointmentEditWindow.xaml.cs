using GlowBook.Model.Data;
using GlowBook.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GlowBook.Wpf.Views.Windows
{
    /// <summary>
    /// Interaction logic for AppointmentEditWindow.xaml
    /// </summary>
    public partial class AppointmentEditWindow : Window
    {
        private readonly int _id;
        private readonly AppDbContext _db;
        private Appointment _appointment = null!;
        public AppointmentEditWindow(int appointmentId)
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

            // afspraak plus eerste dienst
            _appointment = _db.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Staff)
                .Include(a => a.AppointmentServices).ThenInclude(x => x.Service)
                .First(a => a.Id == _id);

            CmbCustomer.SelectedValue = _appointment.CustomerId;
            CmbStaff.SelectedValue = _appointment.StaffId;

            var firstSvc = _appointment.AppointmentServices.FirstOrDefault();
            if (firstSvc != null)
            {
                CmbService.SelectedValue = firstSvc.ServiceId;
                TxtDuration.Text = firstSvc.Service.DurationMin.ToString();
            }

            DpDate.SelectedDate = _appointment.Start.Date;
            TxtStart.Text = _appointment.Start.ToString("HH\\:mm");
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DpDate.SelectedDate == null)
                {
                    MessageBox.Show("Kies een datum."); return;
                }
                if (!TimeSpan.TryParse(TxtStart.Text, out var startTime))
                {
                    MessageBox.Show("Ongeldige starttijd (bv. 09:30)."); return;
                }
                if (!int.TryParse(TxtDuration.Text, out var durMin)) durMin = 60;

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

                // CONFLCTDETECTIE: overlappende afspraken voor dezelfde medewerker
                bool conflict = _db.Appointments
                    .Where(a => !a.IsDeleted && a.StaffId == staffId && a.Id != _appointment.Id)
                    .Any(a => a.Start < end && start < a.End);

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

                var link = _db.AppointmentServices
                              .FirstOrDefault(x => x.AppointmentId == _appointment.Id);
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

                _db.SaveChanges();
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bewaren mislukt: " + ex.Message, "GlowBook", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
