using GlowBook.Model.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;

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

            // Soft delete global filters
            b.Entity<Customer>().HasQueryFilter(x => !x.IsDeleted);
            b.Entity<Staff>().HasQueryFilter(x => !x.IsDeleted);
            b.Entity<Service>().HasQueryFilter(x => !x.IsDeleted);
            b.Entity<Appointment>().HasQueryFilter(x => !x.IsDeleted);
            b.Entity<AppointmentService>().HasQueryFilter(x => !x.IsDeleted);

            b.Entity<Appointment>()
                .HasOne(a => a.Customer).WithMany().HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
            b.Entity<Appointment>()
                .HasOne(a => a.Staff).WithMany().HasForeignKey(a => a.StaffId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed basis via aparte klasse
            Seed.Apply(b);
        }

        // Soft-delete intercept: SaveChanges zet IsDeleted i.p.v. verwijderen
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

