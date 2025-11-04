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

