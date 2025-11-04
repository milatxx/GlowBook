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
            // Rollen
            var adminRole = new IdentityRole { Id = "role-admin", Name = "Admin", NormalizedName = "ADMIN" };
            var empRole = new IdentityRole { Id = "role-employee", Name = "Employee", NormalizedName = "EMPLOYEE" };
            b.Entity<IdentityRole>().HasData(adminRole, empRole);

            // Users
            var hasher = new PasswordHasher<ApplicationUser>();
            var admin = new ApplicationUser
            {
                Id = "user-admin",
                UserName = "admin@glowbook.local",
                NormalizedUserName = "ADMIN@GLOWBOOK.LOCAL",
                Email = "admin@glowbook.local",
                NormalizedEmail = "ADMIN@GLOWBOOK.LOCAL",
                EmailConfirmed = true,
                DisplayName = "Beheerder",
                SecurityStamp = Guid.NewGuid().ToString("D")
            };
            admin.PasswordHash = hasher.HashPassword(admin, "Admin123!");

            var emp = new ApplicationUser
            {
                Id = "user-employee",
                UserName = "employee@glowbook.local",
                NormalizedUserName = "EMPLOYEE@GLOWBOOK.LOCAL",
                Email = "employee@glowbook.local",
                NormalizedEmail = "EMPLOYEE@GLOWBOOK.LOCAL",
                EmailConfirmed = true,
                DisplayName = "Medewerker",
                SecurityStamp = Guid.NewGuid().ToString("D")
            };
            emp.PasswordHash = hasher.HashPassword(emp, "Employee123!");

            b.Entity<ApplicationUser>().HasData(admin, emp);
            b.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> { UserId = "user-admin", RoleId = "role-admin" },
                new IdentityUserRole<string> { UserId = "user-employee", RoleId = "role-employee" }
            );

            // Basisdata
            b.Entity<Service>().HasData(
                new Service { Id = 1, Name = "Wimperlift", DurationMin = 60, Price = 55 },
                new Service { Id = 2, Name = "Manicure", DurationMin = 45, Price = 40 },
                new Service { Id = 3, Name = "Pedicure", DurationMin = 60, Price = 50 }
            );

            b.Entity<Staff>().HasData(
                new Staff { Id = 1, Name = "Tamara", Role = "Beautician", IsActive = true }
            );

            b.Entity<Customer>().HasData(
                new Customer { Id = 1, Name = "Anna V.", Email = "anna@example.com" }
            );
        }
    }
}

