using GlowBook.Model.Data;
using GlowBook.Model.Entities;
using GlowBook.Wpf.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Linq;
using System.Windows;
using Microsoft.AspNetCore.Identity;     
using System.Threading.Tasks;


namespace GlowBook.Wpf.ViewModels
{
    public class StaffViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Staff> Items { get; } = new();
        private Staff? _selectedStaff;
        public Staff? SelectedStaff
        {
            get => _selectedStaff;
            set { _selectedStaff = value; OnPropertyChanged(); CommandManager.InvalidateRequerySuggested(); }
        }

        public ICommand ReloadCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand MakeAdminCommand { get; }    
        public ICommand MakeEmployeeCommand { get; }
        private AppDbContext NewDb() => App.Services.GetRequiredService<AppDbContext>();
        private readonly UserManager<ApplicationUser> _userMgr 
           = App.Services.GetRequiredService<UserManager<ApplicationUser>>();

        public StaffViewModel()
        {

            ReloadCommand = new RelayCommand(_ => Load());
            AddCommand = new RelayCommand(_ => Add());
            SaveCommand = new RelayCommand(_ => { if (SelectedStaff != null) Save(SelectedStaff); });
            DeleteCommand = new RelayCommand(_ => { if (SelectedStaff != null) { SoftDelete(SelectedStaff); Items.Remove(SelectedStaff); SelectedStaff = null; } }, _ => SelectedStaff != null);

            MakeAdminCommand = new RelayCommand(async _ => await SetRoleAsync("Admin", true), _ => SelectedStaff != null);  
            MakeEmployeeCommand = new RelayCommand(async _ => await SetRoleAsync("Employee", true), _ => SelectedStaff != null);

            Load();
        }

        public void Load()
        {
            Items.Clear();
            using var db = NewDb();
            foreach (var s in db.Staff.AsNoTracking().Where(x => !x.IsDeleted).OrderBy(x => x.Name))
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

        public async Task SetRoleAsync(string role, bool enabled)   
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

            var result = enabled                                              
                ? await _userMgr.AddToRoleAsync(user, role)                   
                : await _userMgr.RemoveFromRoleAsync(user, role);             

            if (!result.Succeeded)                                           
                MessageBox.Show("Rolwijziging mislukt: " + string.Join(", ", result.Errors.Select(e => e.Description))); 
            else                                                              
                MessageBox.Show($"Rol '{role}' {(enabled ? "toegevoegd" : "verwijderd")}."); 
        }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
