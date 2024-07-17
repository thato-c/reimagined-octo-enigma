using Automotive.Models;

namespace Automotive.Interfaces
{
    public interface IServiceRepository:IDisposable
    {
        IQueryable<Service> GetServices();
        Task<Service> GetServiceByIdAsync(Guid serviceId);
        void InsertService (Service service);
        Task<Service> DeleteService(Guid serviceId);
        void UpdateService(Service service);
        void SetOriginalRowVersion(Service service, byte[] rowVersion);
        void Save();
        Task SaveAsync();
    }
}
