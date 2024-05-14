using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarDealerAPI.Models;
using CarDealerAPI.Data;
using CarDealerAPI.DTOs;
namespace CarDealerAPI.Controller;

[ApiController]
[Route("api/[controller]")]
public class ModelsController : ControllerBase
{
    private readonly CarDealerContext _context;

    public ModelsController(CarDealerContext context)
    {
        _context = context;
    }

    // GET: api/Models
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ModelDTO>>> GetModels()
    {
        var models = await _context.Models
            .Include(m => m.Manufacturer)
            .Select(m => new ModelDTO
            {
                Id = m.Id,
                Name = m.Name,
                ManufacturerId = m.ManufacturerId,
                Manufacturer = new ManufacturerDTO
                {
                    Id = m.Manufacturer.Id,
                    Name = m.Manufacturer.Name
                }
            })
            .ToListAsync();

        return Ok(models);
    }

    // GET: api/Models/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ModelDTO>> GetModel(int id)
    {
        var model = await _context.Models
            .Include(m => m.Manufacturer)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (model == null)
        {
            return NotFound();
        }

        var modelDTO = new ModelDTO
        {
            Id = model.Id,
            Name = model.Name,
            ManufacturerId = model.ManufacturerId,
            Manufacturer = new ManufacturerDTO
            {
                Id = model.Manufacturer.Id,
                Name = model.Manufacturer.Name
            }
        };

        return Ok(modelDTO);
    }

    // POST: api/Models
    [HttpPost]
    public async Task<ActionResult<ModelDTO>> CreateModel(ModelDTO modelDTO)
    {
        if (modelDTO == null || modelDTO.Name == null)
        {
            return BadRequest("Model name is required.");
        }

        var model = new Model
        {
            Name = modelDTO.Name,
            ManufacturerId = modelDTO.ManufacturerId
        };

        _context.Models.Add(model);
        await _context.SaveChangesAsync();

        modelDTO.Id = model.Id;

        return CreatedAtAction(nameof(GetModel), new { id = model.Id }, modelDTO);
    }

    // PUT: api/Models/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateModel(int id, ModelDTO modelDTO)
    {
        if (id != modelDTO.Id)
        {
            return BadRequest("Model ID mismatch.");
        }

        if (!_context.Manufacturers.Any(m => m.Id == modelDTO.ManufacturerId))
        {
            return BadRequest("Invalid ManufacturerId.");
        }

        var model = await _context.Models.FindAsync(id);
        if (model == null)
        {
            return NotFound();
        }
        
        model.Name = modelDTO.Name ?? model.Name;
        model.ManufacturerId = modelDTO.ManufacturerId;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ModelExists(id))
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

    // DELETE: api/Models/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteModel(int id)
    {
        var model = await _context.Models.FindAsync(id);
        if (model == null)
        {
            return NotFound();
        }

        _context.Models.Remove(model);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ModelExists(int id)
    {
        return _context.Models.Any(m => m.Id == id);
    }
}