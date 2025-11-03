using GlowBook.Model.Data;
using GlowBook.Model.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
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

            using(var scope = Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await db.Database.MigrateAsync();
                await SeedAsync(scope.ServiceProvider);
            }

            // splash -> login
            var splash = new Views.SplashScreen();
            splash.Show();
            splash.Close();

            var login = new Views.LoginWindow();
            login.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            HostApp.Dispose();
            base.OnExit(e);
        }
                    
        // Minimalistische seeding (rollen + admin + demodata))
        private static async Task SeedAsync(IServiceProvider sp)
        {
            using var scope = sp.CreateScope();
            var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (!await roleMgr.RoleExistsAsync("Admin")) await roleMgr.CreateAsync(new IdentityRole("Admin"));
            if (!await roleMgr.RoleExistsAsync("User")) await roleMgr.CreateAsync(new IdentityRole("User"));

            var admin = await userMgr.FindByNameAsync("admin@glowbook");
            if (admin == null)
            {
                admin = new ApplicationUser { UserName = "admin@glowbook", Email = "admin@glowbook" };
                await userMgr.CreateAsync(admin, "Admin123!");
                await userMgr.AddToRoleAsync(admin, "Admin");
            }

            if (!db.Customers.Any())
            {
                db.Customers.Add(new Customer { Name = "Anna V.", Email = "anna@example.com" });
                db.Staff.Add(new Staff { Name = "Tamara", Role = "Beautician" });
                db.Services.Add(new Service { Name = "Wimperlift", DurationMin = 60, Price = 65m });
                await db.SaveChangesAsync();
            }
        }
    }
}



