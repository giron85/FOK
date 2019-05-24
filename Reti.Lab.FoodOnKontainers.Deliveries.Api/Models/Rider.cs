using GeoAPI.Geometries;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Reti.Lab.FoodOnKontainers.Deliveries.Api.Models
{
    public partial class Rider
    {
        public Rider()
        {
            Delivery = new HashSet<Delivery>();
        }

        public int IdRider { get; set; }
        public string RiderName { get; set; }
        [JsonIgnore]
        public IGeometry StartingPoint { get; set; }
        public int? Range { get; set; }
        public decimal? AverageRating { get; set; }
        public bool Active { get; set; }

        public ICollection<Delivery> Delivery { get; set; }

        public DTO.Position StartingPointCoordinates
        {
            get
            {
                return StartingPoint != null
                    ? new DTO.Position()
                    {
                        Latitude = StartingPoint.Coordinate.Y,
                        Longitude = StartingPoint.Coordinate.X
                    }
                    : null;
            }
        }
    }
}
