
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
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LoginWindow()
        {
            InitializeComponent();
            _userManager = App.Services.GetRequiredService<UserManager<ApplicationUser>>();
            _signInManager = App.Services.GetRequiredService<SignInManager<ApplicationUser>>();
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

                
                var result = await _signInManager.CheckPasswordSignInAsync(
                    user, pass, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    // voor Identity-flow
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    var win = new MainWindow(user);
                    Application.Current.MainWindow = win;
                    win.Show();
                    Close();
                    return;
                }

                if (result.IsLockedOut)
                {
                    Error("Account tijdelijk geblokkeerd door te veel mislukte pogingen.");
                    return;
                }

                if (result.IsNotAllowed)
                {
                    Warn("Je account mag nog niet inlogen (bv. e-mail niet bevestigd).");
                    return;
                }

                // Default
                Warn("Wachtwoord onjuist.");
            }
            catch (System.Exception ex)
            {
                Error($"Fout bij inloggen: {ex.Message}");
            }
        }
    }




}