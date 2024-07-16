namespace Automotive.Models
{
    public class VehicleService
    {
        public Guid VehicleServiceId { get; set; }

        public Guid VehicleId { get; set; }

        public Guid ServiceId { get; set; }

        public Vehicle Vehicle { get; set; } = new Vehicle();

        public Service Service { get; set; } = new Service();
    }
}
