
using GlowBook.Model.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using static GlowBook.Wpf.Services.DialogService;

namespace GlowBook.Wpf.Views
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public LoginWindow()
        {
            InitializeComponent();
            _userManager = App.Services.GetRequiredService<UserManager<ApplicationUser>>();
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var login = UserBox.Text?.Trim();
                var pass = PwdBox.Password;

          
                var user = await _userManager.FindByEmailAsync(login)
                           ?? await _userManager.FindByNameAsync(login);

                if (user == null)
                {
                    Warn("Onbekende gebruiker.");
                    return;
                }

                var ok = await _userManager.CheckPasswordAsync(user, pass);
                if (!ok)
                {
                    Warn("Wachtwoord onjuist.");
                    return;
                }

                var win = new MainWindow(user);
                win.Show();
                Close();
            }
            catch (System.Exception ex)
            {
                Error($"Fout bij inloggen: {ex.Message}");
            }
        }
    }
}
