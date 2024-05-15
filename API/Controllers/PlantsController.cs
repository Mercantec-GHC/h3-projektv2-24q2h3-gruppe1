using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Models;

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
            Plant plantCreate = new Plant();

            plantCreate.PlantName = plantCreated.PlantName;
            plantCreate.MinWaterLevel = plantCreated.MinWaterLevel;
            plantCreate.MaxWaterLevel = plantCreated.MaxWaterLevel;

            // is not inf
            plantCreate.UpdatedAt = DateTime.UtcNow;
            plantCreate.CreatedAt = DateTime.UtcNow;
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
        public async Task<IActionResult> PutPlant(int id, Plant plant)
        {
            if (id != plant.Id)
            {
                return BadRequest();
            }

            _context.Entry(plant).State = EntityState.Modified;

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
