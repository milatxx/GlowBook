using GlowBook.Model.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using System;
using GlowBook.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace GlowBook.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IHost HostApp { get; private set; } = null!;
        public static IServiceProvider Services => HostApp.Services;

        protected override void OnStartup(StartupEventArgs e)
        {
            HostApp = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddDbContext<AppDbContext>(opt =>
                        opt.UseSqlite("Data Source=glowbook.db"));

                    services.AddIdentityCore<ApplicationUser>(o =>
                    {
                        o.Password.RequireDigit = false;
                        o.Password.RequireNonAlphanumeric = false;
                        o.Password.RequiredLength = 6;
                    })
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<AppDbContext>();

                    services.AddScoped<UserManager<ApplicationUser>>();
                    services.AddScoped<SignInManager<ApplicationUser>>();
                })
                .Build();

            // da DB exists/migrated
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            HostApp.Dispose();
            base.OnExit(e);
        }
    }
}
