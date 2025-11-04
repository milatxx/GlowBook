using GlowBook.Model.Data;
using GlowBook.Model.Entities;
using GlowBook.Wpf.Views;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
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

            try
            {
                using var scope2 = Services.CreateScope();
                var roleManager = scope2.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope2.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                // rollen
                if (!await roleManager.RoleExistsAsync("Admin"))
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                if (!await roleManager.RoleExistsAsync("Employee"))
                    await roleManager.CreateAsync(new IdentityRole("Employee"));

                // admin
                var admin = await userManager.FindByEmailAsync("admin@glowbook.local");
                if (admin == null)
                {
                    admin = new ApplicationUser
                    {
                        UserName = "admin@glowbook.local",
                        Email = "admin@glowbook.local",
                        EmailConfirmed = true,
                        DisplayName = "Beheerder",
                        IsActive = true
                    };
                    await userManager.CreateAsync(admin, "Admin123!");
                    await userManager.AddToRoleAsync(admin, "Admin");
                }

                // employee
                var emp = await userManager.FindByEmailAsync("employee@glowbook.local");
                if (emp == null)
                {
                    emp = new ApplicationUser
                    {
                        UserName = "employee@glowbook.local",
                        Email = "employee@glowbook.local",
                        EmailConfirmed = true,
                        DisplayName = "Medewerker",
                        IsActive = true
                    };
                    await userManager.CreateAsync(emp, "Employee123!");
                    await userManager.AddToRoleAsync(emp, "Employee");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Seeden van users/rollen mislukt: {ex.Message}", "Seed fout",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }


            // splash -> login -> main
            try
            { 
                var splash = new Views.SplashScreen();
                splash.Show();
                splash.Close();

                var login = new Views.LoginWindow();
                var ok = login.ShowDialog() == true;

                if (!ok)
                {
                    Shutdown();
                    return;
                }

                var main = new MainWindow(login.AuthenticatedUser!);         
                Application.Current.MainWindow = main;
                main.Show();

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



