using GlowBook.Model.Data;
using GlowBook.Model.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Windows;
using static GlowBook.Wpf.Services.DialogService;

namespace GlowBook.Wpf.Views
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly SignInManager<ApplicationUser> _signIn;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _db;
        public LoginWindow()
        {
            InitializeComponent();
            var sp = App.Services;
            _db = sp.GetRequiredService<AppDbContext>();
            _signIn = sp.GetRequiredService<SignInManager<ApplicationUser>>();
            _userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(UserBox.Text);
                if (user == null) { Warn("Onbekende gebruiker."); return; }

                var res = await _signIn.CheckPasswordSignInAsync(user, PwdBox.Password, false);
                if (!res.Succeeded) { Warn("Wachtwoord onjuist."); return; }

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
