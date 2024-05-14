using CarDealerAPI.DTOs;

namespace CarDealerAPI.DTOs
{
    public class ModelDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int ManufacturerId { get; set; }
        public ManufacturerDTO? Manufacturer { get; set; }
    }
}
