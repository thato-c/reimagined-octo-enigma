using System.ComponentModel.DataAnnotations;

namespace Automotive.Models
{
    public class Vehicle
    {
        public Guid VehicleId { get; set; }

        public string LicensePlate { get; set; } = string.Empty;

        public int Year { get; set; } = 0;

        public int VIN { get; set; } = 0;

        public int Mileage { get; set; } = 0;

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public ICollection<VehicleService> VehicleServices { get; set; } = new List<VehicleService>();
    }
}
