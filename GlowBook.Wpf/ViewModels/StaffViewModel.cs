using GlowBook.Model.Data;
using GlowBook.Model.Entities;
using GlowBook.Wpf.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GlowBook.Wpf.ViewModels
{
    public class StaffViewModel : INotifyPropertyChanged
    {
        // Data
        public ObservableCollection<Staff> Items { get; } = new();

        private Staff? _selectedStaff;
        public Staff? SelectedStaff
        {
            get => _selectedStaff;
            set { _selectedStaff = value; OnPropertyChanged(); CommandManager.InvalidateRequerySuggested(); }
        }

        // Alleen admin mag rollen wjijzigen
        private bool _isAdminUi;
        public bool IsAdminUi
        {
            get => _isAdminUi;
            set { _isAdminUi = value; OnPropertyChanged(); }
        }

        
        public ICommand ReloadCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ToggleAdminCommand { get; }
        public ICommand ToggleEmployeeCommand { get; }

        // Services
        private AppDbContext NewDb() => App.Services.GetRequiredService<AppDbContext>();
        private readonly UserManager<ApplicationUser> _userMgr
            = App.Services.GetRequiredService<UserManager<ApplicationUser>>();

        public StaffViewModel()
        {
            ReloadCommand = new RelayCommand(_ => Load());
            AddCommand = new RelayCommand(_ => Add());
            SaveCommand = new RelayCommand(_ => Save(SelectedStaff!),_ => SelectedStaff != null);
            DeleteCommand = new RelayCommand(_ => { if (SelectedStaff != null) { SoftDelete(SelectedStaff); Items.Remove(SelectedStaff); SelectedStaff = null; } }, _ => SelectedStaff != null);

            
            ToggleAdminCommand = new RelayCommand(async _ => await ToggleRoleAsync("Admin"), _ => SelectedStaff != null);
            ToggleEmployeeCommand = new RelayCommand(async _ => await ToggleRoleAsync("Employee"), _ => SelectedStaff != null);

            Load();
            _ = InitAdminFlagAsync(); 
        }

        // Ingelogde gebruiker is Admin?
        private async Task InitAdminFlagAsync()
        {
            var current = AuthSession.CurrentUser; 
            if (current == null)
            {
                IsAdminUi = false;
                return;
            }
            var roles = await _userMgr.GetRolesAsync(current);
            IsAdminUi = roles.Contains("Admin");
        }

        //crud
        public void Load()
        {
            Items.Clear();
            using var db = NewDb();
            foreach (var s in db.Staff.AsNoTracking().OrderBy(x => x.Name))
                Items.Add(s);

            SelectedStaff = Items.FirstOrDefault();
        }

        public void Save(Staff s)
        {
            using var db = NewDb();
            if (s.Id == 0) db.Staff.Add(s); else db.Staff.Update(s);
            db.SaveChanges();
        }

        public void SoftDelete(Staff s)
        {
            using var db = NewDb();
            var entity = db.Staff.First(x => x.Id == s.Id);
            entity.IsDeleted = true;
            db.SaveChanges();
        }

        private void Add()
        {
            var s = new Staff { Name = "Nieuwe medewerker", Role = "Employee" };
            Items.Add(s);
            SelectedStaff = s;
        }

        // Rollen beheren
        public async Task ToggleRoleAsync(string role)
        {
            if (SelectedStaff == null)
            {
                MessageBox.Show("Geen medewerker geselecteerd.");
                return;
            }
            if (string.IsNullOrWhiteSpace(SelectedStaff.Email))
            {
                MessageBox.Show("Geef een e-mail op bij de medewerker (moet gelijk zijn aan de Identity-gebruiker).");
                return;
            }

            var user = await _userMgr.FindByEmailAsync(SelectedStaff.Email);
            if (user == null)
            {
                MessageBox.Show("Geen Identity-gebruiker gevonden met dit e-mailadres.");
                return;
            }

            var roles = await _userMgr.GetRolesAsync(user);
            bool heeftRol = roles.Contains(role);

            var result = heeftRol
                ? await _userMgr.RemoveFromRoleAsync(user, role)
                : await _userMgr.AddToRoleAsync(user, role);

            if (!result.Succeeded)
            {
                MessageBox.Show("Rolwijziging mislukt: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                return;
            }

            // Update lokale weergave
            if (role == "Admin")
                SelectedStaff.Role = heeftRol ? "Employee" : "Admin";

            OnPropertyChanged(nameof(SelectedStaff));
            MessageBox.Show($"Rol '{role}' {(heeftRol ? "verwijderd" : "toegevoegd")}.");
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
