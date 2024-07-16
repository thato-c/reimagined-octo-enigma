using Automotive.Models;
using Microsoft.EntityFrameworkCore;

namespace Automotive.Data
{
    public class AutomotiveDBContext:DbContext
    {
        public AutomotiveDBContext(DbContextOptions<AutomotiveDBContext> options) : base(options) 
        { 
        }

        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<VehicleService> VehicleServices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Vehicle>().ToTable("Vehicle");
            modelBuilder.Entity<Service>().ToTable("Service");
            modelBuilder.Entity<VehicleService>().ToTable("VehicleService");

            // Configure the relationship between Vehicle and VehicleService
            modelBuilder.Entity<Vehicle>()
                .HasMany(vehicle => vehicle.VehicleServices)
                .WithOne(VehicleService => VehicleService.Vehicle)
                .HasForeignKey(vehicle => vehicle.VehicleId);

            // Configure the relationship between Service and VehicleService
            modelBuilder.Entity<Service>()
                .HasMany(service => service.VehicleServices)
                .WithOne(VehicleService => VehicleService.Service)
                .HasForeignKey(service => service.ServiceId);
        }
    }
}
