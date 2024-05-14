using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarDealerAPI.Data;
using CarDealerAPI.Models;
using CarDealerAPI.DTOs;

namespace CarDealerAPI.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManufacturersController : ControllerBase
    {
        private readonly CarDealerContext _context;

        public ManufacturersController(CarDealerContext context)
        {
            _context = context;
        }

        // GET: api/Manufacturers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ManufacturerDTO>>> GetManufacturers()
        {
            var manufacturers = await _context.Manufacturers
                .Select(m => new ManufacturerDTO
                {
                    Id = m.Id,
                    Name = m.Name,
                    Models = m.Models.Select(model => new ModelDTO
                    {
                        Id = model.Id,
                        Name = model.Name
                    }).ToList()
                })
                .ToListAsync();

            return manufacturers;
        }

        // GET: api/Manufacturers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ManufacturerDTO>> GetManufacturer(int id)
        {
            var manufacturer = await _context.Manufacturers
                .Select(m => new ManufacturerDTO
                {
                    Id = m.Id,
                    Name = m.Name,
                    Models = m.Models.Select(model => new ModelDTO
                    {
                        Id = model.Id,
                        Name = model.Name
                    }).ToList()
                })
                .FirstOrDefaultAsync(m => m.Id == id);

            if (manufacturer == null)
            {
                return NotFound();
            }

            return manufacturer;
        }

        // POST: api/Manufacturers
        [HttpPost]
        public async Task<ActionResult<Manufacturer>> PostManufacturer(ManufacturerDTO manufacturerDTO)
        {
            var manufacturer = new Manufacturer { Name = manufacturerDTO.Name };
            _context.Manufacturers.Add(manufacturer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetManufacturer), new { id = manufacturer.Id }, manufacturer);
        }

        // PUT: api/Manufacturers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutManufacturer(int id, ManufacturerDTO manufacturerDTO)
        {
            if (id != manufacturerDTO.Id)
            {
                return BadRequest();
            }

            var manufacturer = await _context.Manufacturers.FindAsync(id);
            if (manufacturer == null)
            {
                return NotFound();
            }

            manufacturer.Id = manufacturerDTO.Id;
            manufacturer.Name = manufacturerDTO.Name;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ManufacturerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Manufacturers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteManufacturer(int id)
        {
            var manufacturer = await _context.Manufacturers.FindAsync(id);
            if (manufacturer == null)
            {
                return NotFound();
            }

            _context.Manufacturers.Remove(manufacturer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ManufacturerExists(int id)
        {
            return _context.Manufacturers.Any(m => m.Id == id);
        }
    }

}
