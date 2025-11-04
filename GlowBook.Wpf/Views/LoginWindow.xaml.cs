
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

                if (string.IsNullOrWhiteSpace(login) || string.IsNullOrEmpty(pass))
                {
                    Warn("Vul je e-mail/gebruikersnaam en wachtwoord in.");
                    return;
                }

                
                var user = await _userManager.FindByEmailAsync(login)
                          ?? await _userManager.FindByNameAsync(login);

                if (user == null)
                {
                    Warn("Onbekende gebruiker.");
                    return;
                }

                var ok = await _userManager.CheckPasswordAsync(user, pass);

                if (ok)
                {
                    // faal teller resetten
                    await _userManager.ResetAccessFailedCountAsync(user);

                    var win = new MainWindow(user);
                    Application.Current.MainWindow = win;
                    win.Show();
                    Close();
                    return;
                }
                else
                {
                    // lockout mechanisme bijhouden
                    await _userManager.AccessFailedAsync(user);
                    if (await _userManager.IsLockedOutAsync(user))
                    {
                        Error("Account tijdelijk geblokkeerd door te veel mislukte pogingen.");
                        return;
                    }

                    Warn("Wachtwoord onjuist.");
                }


            }
            catch (System.Exception ex)
            {
                Error($"Fout bij inloggen: {ex.Message}");
            }
        }
    }




}