using GlowBook.Model.Data;
using GlowBook.Model.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Windows;
using System.Linq;
using System.Threading.Tasks;


namespace GlowBook.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IHost HostApp { get; private set; } = null!;
        public static IServiceProvider Services => HostApp.Services;

        private async void OnStartup(object sender, StartupEventArgs e)
        {
            HostApp = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddDbContext<AppDbContext>(opt =>
                    {
                        var dataDir = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                            "GlowBook");
                        Directory.CreateDirectory(dataDir);
                        var dbPath = Path.Combine(dataDir, "glowbook.db");

                        opt.UseSqlite(
                            $"Data Source={dbPath}",
                            b => b.MigrationsAssembly("GlowBook.Model"));
                    });

                    services.AddIdentityCore<ApplicationUser>(o =>
                    {
                        o.Password.RequireDigit = false;
                        o.Password.RequireNonAlphanumeric = false;
                        o.Password.RequireUppercase = false;
                        o.Password.RequireLowercase = false;
                        o.Password.RequiredLength = 6;
                    })
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddSignInManager();


                })
                .Build();

            // DB migreren plus seed
            try
            {
                using var scope = Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await db.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"De databank kon niet worden bijgewerkt:\n{ex.Message}",
                    "Database fout",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                
                Shutdown();
                return;
            }

            // splash -> login
            try
            { 
                var splash = new Views.SplashScreen();
                splash.Show();
                splash.Close();

                var login = new Views.LoginWindow();
                login.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Er trad een fout op tijdens het starten van de UI:\n{ex.Message}",
                    "Startfout",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Shutdown();
            }
        }


        protected override void OnExit(ExitEventArgs e)
        {
            HostApp.Dispose();
            base.OnExit(e);
        }
                    
    }
}



