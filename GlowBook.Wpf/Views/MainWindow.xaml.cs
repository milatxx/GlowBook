using GlowBook.Model.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using GlowBook.Wpf.Views.Pages;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;



namespace GlowBook.Wpf.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ApplicationUser _user;
        private readonly UserManager<ApplicationUser> _userManager;
        public MainWindow(ApplicationUser user)
        {
            InitializeComponent();
            _user = user;
            _userManager = App.Services.GetRequiredService<UserManager<ApplicationUser>>();
            GlowBook.Wpf.Helpers.AuthSession.CurrentUser = _user;
            Loaded += async (_, __) =>
            {
                await ApplyRoleVisibilityAsync();
                MainFrame.Content = new HomeView();
            };
        }

        private async Task ApplyRoleVisibilityAsync()
        {
            try
            {
                if (_user == null)
                {
                    MessageBox.Show("Geen ingelogde gebruiker gevonden. Meld opnieuw aan.", "GlowBook", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                var roles = await _userManager.GetRolesAsync(_user);
                bool isAdmin = roles.Contains("Admin");

                btnReports.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
                // services altijd zichtbaar maar alleen Admins mogen aanpassen
                btnServices.Visibility = Visibility.Visible;
                btnServices.IsEnabled = isAdmin;
                // kalender en klanten zichtbaarr voor beide
                btnCal.Visibility = Visibility.Visible;
                btnCustomers.Visibility = Visibility.Visible;
                btnStaff.Visibility = Visibility.Visible; // tonen in de toolbar
                btnStaff.IsEnabled = isAdmin;            // enkel Admin kan ernaartoe

            }

            catch (System.Exception ex)
            {
                MessageBox.Show($"Fout bij toepassen van rollen: {ex.Message}", "GlowBook", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Home_Click(object s, RoutedEventArgs e) => MainFrame.Content = new HomeView();
        private void Appointments_Click(object s, RoutedEventArgs e) => MainFrame.Content = new AppointmentsPage();
        private void Customers_Click(object s, RoutedEventArgs e) => MainFrame.Content = new CustomersPage();
        private void Services_Click(object s, RoutedEventArgs e) => MainFrame.Content = new ServicesPage();
        private void Staff_Click(object s, RoutedEventArgs e) => MainFrame.Content = new StaffPage();

        private void Reports_Click(object s, RoutedEventArgs e) => MainFrame.Content = new ReportsPage();

        private void Logout_Click(object s, RoutedEventArgs e)
        {
            // login modaal, wacht op resultaat
            var login = new LoginWindow { Owner = this };
            var ok = login.ShowDialog() == true;

            if (!ok)
            return;

            GlowBook.Wpf.Helpers.AuthSession.CurrentUser = login.AuthenticatedUser!;

            // succesvol opnieuw ingelogd: open nieuw mainwinwow met nieuwe user
            var newMain = new MainWindow(login.AuthenticatedUser!);

            // zet nieuwe als mainwindow -> app blijft open
            Application.Current.MainWindow = newMain;
            newMain.Show();

            // sluit oude mainwindow
            Close();
        }
    }
}
