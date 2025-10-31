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
            Loaded += async (_, __) =>
            {
                await ApplyRoleVisibility();
                MainFrame.Content = new HomeView();
            };
        }

        private async Task ApplyRoleVisibility()
        {
            var roles = await _userManager.GetRolesAsync(_user);
            bool isAdmin = roles.Contains("Admin");

            btnReports.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
            btnServices.Visibility = isAdmin ? Visibility.Visible : Visibility.Visible;
            // kalender en klanten zichtbaarr voor beide
        }

        private void Home_Click(object s, RoutedEventArgs e) => MainFrame.Content = new HomeView();
        private void Appointments_Click(object s, RoutedEventArgs e) => MainFrame.Content = new AppointmentsPage();
        private void Customers_Click(object s, RoutedEventArgs e) => MainFrame.Content = new CustomersPage();
        private void Services_Click(object s, RoutedEventArgs e) => MainFrame.Content = new ServicesPage();
        private void Reports_Click(object s, RoutedEventArgs e) => MainFrame.Content = new ReportsPage();

        private void Logout_Click(object s, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            Close();
        }
    }
}
