using Microsoft.EntityFrameworkCore;
using RestaurantsApplication.Data.Entities;

namespace RestaurantsApplication.Data
{
    public class RestaurantsContext : DbContext
    {
        public RestaurantsContext()
        {
        }
        public RestaurantsContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Employment> Employments { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Record> Records { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Request>()
                .HasOne(r => r.Location)
                .WithMany()
                .HasForeignKey(r => r.LocationCode)
                .HasPrincipalKey(l => l.Code);

            modelBuilder.Entity<Record>()
               .HasOne(r => r.Employee)
               .WithMany()
               .HasForeignKey(r => r.EmployeeCode)
               .HasPrincipalKey(e => e.Code);

            SeedRoles(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private void SeedRoles(ModelBuilder builder)
        {
            var roles = new List<Role>()
            {
                new Role{ Id = 1, Name = "Manager"},
                new Role{ Id = 2, Name = "Chef"},
                new Role{ Id = 3, Name = "Bar Server"},
                new Role{ Id = 4, Name = "Waiter"},
                new Role{ Id = 5, Name = "Runner"},
                new Role{ Id = 6, Name = "Dishwasher"},
                new Role{ Id = 7, Name = "Cleaner"},
            };

            builder.Entity<Role>().HasData(roles);
        }
    }
}