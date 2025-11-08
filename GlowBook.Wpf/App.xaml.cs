using GlowBook.Model.Data;
using GlowBook.Model.Entities;
using GlowBook.Wpf.Views;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;


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
            var splash = new Views.SplashScreen();
            splash.Show();

            Debug.WriteLine("Start applicatie!");
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
                    .AddEntityFrameworkStores<AppDbContext>();



                })
                .Build();

            Debug.WriteLine("Databank migratie start");

            // DB migreren plus seed
            try
            {
                using var scope = Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var hasMigrations = (await db.Database.GetPendingMigrationsAsync()).Any();
                if (hasMigrations)
                    await db.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                MessageBox.Show(
                    $"De databank kon niet worden bijgewerkt:\n{ex.Message}",
                    "Database fout",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                
                Shutdown();
                return;
            }

            try
            {
                using var scope = Services.CreateScope();
                var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                async Task EnsureRoleAsync(string name)
                {
                    if (!await roleMgr.RoleExistsAsync(name))
                        await roleMgr.CreateAsync(new IdentityRole(name));
                }

                await EnsureRoleAsync("Admin");
                await EnsureRoleAsync("Employee");

                // admin
                var admin = await userMgr.FindByEmailAsync("admin@glowbook.local");
                if (admin is null)
                {
                    admin = new ApplicationUser
                    {
                        UserName = "admin@glowbook.local",
                        Email = "admin@glowbook.local",
                        EmailConfirmed = true,
                        DisplayName = "Beheerder",
                        IsActive = true
                    };
                    await userMgr.CreateAsync(admin, "Admin123!");

                }
                if (!await userMgr.IsInRoleAsync(admin, "Admin"))
                    await userMgr.AddToRoleAsync(admin, "Admin");
            

                // employee
                var emp = await userMgr.FindByEmailAsync("employee@glowbook.local");
                if (emp is null)
                {
                    emp = new ApplicationUser
                    {
                        UserName = "employee@glowbook.local",
                        Email = "employee@glowbook.local",
                        EmailConfirmed = true,
                        DisplayName = "Medewerker",
                        IsActive = true
                    };
                    await userMgr.CreateAsync(emp, "Employee123!");
                }
                if (!await userMgr.IsInRoleAsync(emp, "Employee"))
                    await userMgr.AddToRoleAsync(emp, "Employee");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                MessageBox.Show($"Seeden van users/rollen mislukt: {ex.Message}", "Seed fout",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }


            // splash -> login -> main
            try
            {
               

                var login = new Views.LoginWindow();

                splash.Close();

                if (!login.ShowDialog() ?? false)
                {
                    Debug.WriteLine("Niet ok, shutdown");
                    Shutdown();
                    return;
                }

                Debug.WriteLine("Ok, main window on the way!");
                var main = new MainWindow(login.AuthenticatedUser);         
                Application.Current.MainWindow = main;
                Application.Current.MainWindow.Show();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
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



