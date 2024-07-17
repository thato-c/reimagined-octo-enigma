using Automotive.Models;

namespace Automotive.Interfaces
{
    public interface IVehicleRepository:IDisposable
    {
        IQueryable<Vehicle> GetVehicles();
        Task<Vehicle> GetVehiclesByIdAsync(Guid vehicleId);
        void InsertVehicle(Vehicle vehicle);
        Task<Vehicle> DeleteVehicle(Guid vehicleId);
        void UpdateVehicle(Vehicle vehicle);
        void SetOriginalRowVersion(Vehicle vehicle, byte[] rowVersion);
        void Save();
        Task SaveAsync();
    }
}
