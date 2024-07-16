using System.ComponentModel.DataAnnotations;

namespace Automotive.Models
{
    public class Service
    {
        public Guid ServiceId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int Price { get; set; } = 0;

        public int LaborHours { get; set; } = 0;

        public int WarrantyInMonths { get; set; } = 0;

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public ICollection<VehicleService> VehicleServices { get; set; } = new List<VehicleService>();
    }
}
