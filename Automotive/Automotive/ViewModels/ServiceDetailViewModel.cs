using System.ComponentModel.DataAnnotations;

namespace Automotive.ViewModels
{
    public class ServiceDetailViewModel
    {
        public Guid ServiceId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        public int Price { get; set; } = 0;

        [Required(ErrorMessage = "Labour Hours afre required")]
        public int LaborHours { get; set; } = 0;

        [Required(ErrorMessage = "Warranty is required.")]
        public int WarrantyInMonths { get; set; } = 0;

        [Timestamp]
        public byte[] RowVersion { get; set; }

    }
}
