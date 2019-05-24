using System.ComponentModel.DataAnnotations;

namespace Reti.Lab.FoodOnKontainers.Deliveries.Api.DTO
{
    public class Position
    {
        [Required]
        public double Longitude { get; set; }
        [Required]
        public double Latitude { get; set; }
    }
}
