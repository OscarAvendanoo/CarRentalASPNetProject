using FribergCarRentals.Models;
using Microsoft.EntityFrameworkCore;

namespace FribergCarRentals.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Behövde göra det här för att databasen ska veta hur den ska hantera typen decimal
            modelBuilder.Entity<Booking>()
                .Property(b => b.TotalCost)
                .HasColumnType("decimal(6, 2)");  // Innebär att den kan spara tal upp till en miljon med 2 siffror efter kommatecknet
        }
    }
}
