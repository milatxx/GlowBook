using GlowBook.Model.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;


namespace GlowBook.Model.Data
{
    public static class Seed
    {
        public static void Apply(ModelBuilder b)
        {
            // Services
            b.Entity<Service>().HasData(
                new Service { Id = 1, Name = "Wimperlift", DurationMin = 60, Price = 55 },
                new Service { Id = 2, Name = "Manicure", DurationMin = 45, Price = 40 },
                new Service { Id = 3, Name = "Gezichtsbehandeling", DurationMin = 75, Price = 85 }
            );
            // Staf
            b.Entity<Staff>().HasData(
                new Staff { Id = 1, Name = "Tamara", Role = "Employee" },
                new Staff { Id = 2, Name = "Radmila", Role = "Admin" }
            );
            // Customers
            b.Entity<Customer>().HasData(
                new Customer { Id = 1, Name = "Amina V.", Email = "amina.v@gmail.com" },
                new Customer { Id = 2, Name = "Sarah K." },
                new Customer { Id = 3, Name = "Lina B." }
            );
            // Afspraken (dummy)
            var now = DateTime.Today.AddHours(9);
            b.Entity<Appointment>().HasData(
                new Appointment { Id = 1, CustomerId = 1, StaffId = 1, Start = now, End = now.AddMinutes(60), Status = "Planned" },
                new Appointment { Id = 2, CustomerId = 2, StaffId = 2, Start = now.AddHours(1.5), End = now.AddHours(2.75), Status = "Planned" }
            );
            b.Entity<AppointmentService>().HasData(
                new AppointmentService { Id = 1, AppointmentId = 1, ServiceId = 3, Qty = 1 },
                new AppointmentService { Id = 2, AppointmentId = 2, ServiceId = 1, Qty = 1 }
            );

            // Identity seeds (rollen + 2 users)
            var adminRole = new IdentityRole("Admin") { NormalizedName = "ADMIN" };
            var empRole = new IdentityRole("Employee") { NormalizedName = "EMPLOYEE" };
            b.Entity<IdentityRole>().HasData(adminRole, empRole);

            var hasher = new PasswordHasher<ApplicationUser>();
            var admin = new ApplicationUser
            {
                Id = "u1",
                UserName = "admin@glowbook",
                NormalizedUserName = "ADMIN@GLOWBOOK",
                Email = "admin@glowbook",
                NormalizedEmail = "ADMIN@GLOWBOOK",
                DisplayName = "Admin",
                SecurityStamp = Guid.NewGuid().ToString("D"),
                EmailConfirmed = true
            };
            admin.PasswordHash = hasher.HashPassword(admin, "Admin123!");

            var emp = new ApplicationUser
            {
                Id = "u2",
                UserName = "medewerker@glowbook",
                NormalizedUserName = "MEDEWERKER@GLOWBOOK",
                Email = "medewerker@glowbook",
                NormalizedEmail = "MEDEWERKER@GLOWBOOK",
                DisplayName = "Medewerker",
                SecurityStamp = Guid.NewGuid().ToString("D"),
                EmailConfirmed = true
            };
            emp.PasswordHash = hasher.HashPassword(emp, "Employee123!");

            b.Entity<ApplicationUser>().HasData(admin, emp);
            b.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> { UserId = "u1", RoleId = adminRole.Id },
                new IdentityUserRole<string> { UserId = "u2", RoleId = empRole.Id }
            );
        }
    }
}
