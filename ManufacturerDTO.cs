
using System.Collections.Generic;

namespace CarDealerAPI.DTOs
{
    public class ManufacturerDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public ICollection<ModelDTO>? Models { get; set; }
    }
}

