using Automotive.Data;
using Automotive.Interfaces;
using Automotive.Models;
using Microsoft.EntityFrameworkCore;

namespace Automotive.Repositories
{
    public class VehicleRepository:IVehicleRepository, IDisposable
    {
        private readonly AutomotiveDBContext context;

        public VehicleRepository(AutomotiveDBContext context)
        {
            this.context = context;
        }

        public IQueryable<Vehicle> GetVehicles() 
        {
            return context.Vehicles.AsQueryable();
        }

        public async Task<Vehicle> GetVehiclesByIdAsync(Guid vehicleId)
        {
            return await context.Vehicles
                        .AsNoTracking()
                        .FirstOrDefaultAsync(v => v.VehicleId == vehicleId);
        }

        public void InsertVehicle(Vehicle vehicle)
        {
            context.Vehicles.Add(vehicle);
        }

        public async Task<Vehicle> DeleteVehicle(Guid vehicleId)
        {
            var vehicle = await context.Vehicles.FindAsync(vehicleId);

            if (vehicle != null)
            {
                context.Vehicles.Remove(vehicle);
            }

            return null;
        }

        public void UpdateVehicle(Vehicle vehicle)
        {
            context.Entry(vehicle).State = EntityState.Modified;
        }

        public void SetOriginalRowVersion(Vehicle vehicle, byte[] rowVersion)
        {
            context.Entry(vehicle).Property("RowVersion").OriginalValue = rowVersion;
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true); 
            GC.SuppressFinalize(this);
        }

    }
}
