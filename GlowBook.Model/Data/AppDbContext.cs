using GlowBook.Model.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace GlowBook.Model.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Staff> Staff => Set<Staff>();
        public DbSet<Service> Services => Set<Service>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<AppointmentService> AppointmentServices => Set<AppointmentService>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // Relaties
            b.Entity<AppointmentService>()
                .HasOne(x => x.Appointment)
                .WithMany(a => a.AppointmentServices)
                .HasForeignKey(x => x.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<AppointmentService>()
                .HasOne(x => x.Service)
                .WithMany(s => s.AppointmentServices)
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<Appointment>()
                .HasOne(a => a.Customer)
                .WithMany(c => c.Appointments)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<Appointment>()
                .HasOne(a => a.Staff)
                .WithMany(s => s.Appointments)
                .HasForeignKey(a => a.StaffId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unieke combinaties
            b.Entity<AppointmentService>()
                .HasIndex(x => new { x.AppointmentId, x.ServiceId })
                .IsUnique();

            b.Entity<Customer>().HasQueryFilter(x => !x.IsDeleted);
            b.Entity<Staff>().HasQueryFilter(x => !x.IsDeleted);
            b.Entity<Service>().HasQueryFilter(x => !x.IsDeleted);
            b.Entity<Appointment>().HasQueryFilter(x => !x.IsDeleted);
            b.Entity<AppointmentService>().HasQueryFilter(x => !x.IsDeleted);

            // Decimal precisie
            b.Entity<Service>()
            .Property(s => s.Price)
            .HasPrecision(10, 2);


            // Seed basis via aparte klasse
            Seed.Apply(b);
        }

        // Soft delete override
        public override int SaveChanges()
        {
            foreach (var e in ChangeTracker.Entries<BaseEntity>()
                         .Where(e => e.State == EntityState.Deleted))
            {
                e.State = EntityState.Modified;
                e.Entity.IsDeleted = true;
            }
            return base.SaveChanges();
        }
        public override Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            foreach (var e in ChangeTracker.Entries<BaseEntity>()
                         .Where(e => e.State == EntityState.Deleted))
            {
                e.State = EntityState.Modified;
                e.Entity.IsDeleted = true;
            }
            return base.SaveChangesAsync(ct);
        }
    }
}

