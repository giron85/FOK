using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reti.Lab.FoodOnKontainers.Reviews.Api.DTO
{
    public class RiderReviewDto
    {
        public int IdOrder { get; set; }
        public int IdRider { get; set; }
        public string RyderName { get; set; }
        public int IdUser { get; set; }
        public string UserName { get; set; }
        public string Review { get; set; }
        public short Rating { get; set; }
    }
}
