using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlantsController : ControllerBase
    {
        private readonly AppDBContext _context;

        public PlantsController(AppDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Plant>>> GetPlants()
        {
            return await _context.Plants.ToListAsync();
        }

        // ---------------- Plant Credentials ------------------ //

        [HttpPost]
        public async Task<ActionResult<Plant>> PostPlant(PlantCreation plantCreated)
        {
            Plant plantCreate = new Plant
            {
                PlantName = plantCreated.PlantName,
                UserId = plantCreated.UserId,
                MinWaterLevel = plantCreated.MinWaterLevel,
                MaxWaterLevel = plantCreated.MaxWaterLevel,

                UpdatedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            _context.Plants.Add(plantCreate);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlants", new { id = plantCreate.Id }, plantCreate);
        }

        // ----------------------- ID -------------------------- //

        [HttpGet("{id}")]
        public async Task<ActionResult<Plant>> GetPlant(int id)
        {
            var plant = await _context.Plants.FindAsync(id);

            if (plant == null)
            {
                return NotFound();
            }

            return plant;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlant(int id, EditPlant editPlant)
        {
            var existingPlant = await _context.Plants.FindAsync(id);
            if (existingPlant == null)
            {
                return NotFound();
            }

            // Update properties
            existingPlant.MinWaterLevel = editPlant.MinWaterLevel;
            existingPlant.MaxWaterLevel = editPlant.MaxWaterLevel;
            existingPlant.PlantName = editPlant.PlantName;
        
            _context.Entry(existingPlant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlantExists(id))
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



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlant(int id)
        {
            var plant = await _context.Plants.FindAsync(id);
            if (plant == null)
            {
                return NotFound();
            }

            _context.Plants.Remove(plant);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ----------------------- Mis ------------------------ //

        private bool PlantExists(int id)
        {
            return _context.Plants.Any(e => e.Id == id);
        }
    }
}
