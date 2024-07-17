using Automotive.Data;
using Automotive.Interfaces;
using Automotive.Models;
using Microsoft.EntityFrameworkCore;

namespace Automotive.Repositories
{
    public class ServiceRepository : IServiceRepository, IDisposable
    {
        private readonly AutomotiveDBContext context;

        public ServiceRepository(AutomotiveDBContext context)
        {
            this.context = context;
        }

        public async Task<Service> DeleteService(Guid serviceId)
        {
            var service = await context.Services.FindAsync(serviceId);

            if (service != null)
            {
                context.Services.Remove(service);
            }

            return null;
        }

        public async Task<Service> GetServiceByIdAsync(Guid serviceId)
        {
            return await context.Services
                        .AsNoTracking()
                        .FirstOrDefaultAsync(s => s.ServiceId == serviceId);
        }

        public IQueryable<Service> GetServices()
        {
            return context.Services.AsQueryable();
        }

        public void InsertService(Service service)
        {
            context.Services.Add(service);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }

        public void SetOriginalRowVersion(Service service, byte[] rowVersion)
        {
            context.Entry(service).Property("RowVersion").OriginalValue = rowVersion;
        }

        public void UpdateService(Service service)
        {
            context.Entry(service).State = EntityState.Modified;
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
