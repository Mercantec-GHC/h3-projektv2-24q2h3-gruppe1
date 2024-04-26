using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Models;

// Planted createdTime = createdTime DateTime.UtcNow

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

        // GET: api/PlantOverviews
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Plant>>> GetPlants()
        {
            return await _context.Plants.ToListAsync();
        }

        // GET: api/PlantOverviews/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Plant>> GetPlantOverview(int id)
        {
            var plant = await _context.Plants.FindAsync(id);

            if (plant == null)
            {
                return NotFound();
            }

            return plant;
        }

        // PUT: api/PlantOverviews/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlantOverview(int id, Plant plantOverview)
        {
            if (id != plantOverview.Id)
            {
                return BadRequest();
            }

            _context.Entry(plantOverview).State = EntityState.Modified;

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

        // POST: api/PlantOverviews
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Plant>> PostPlant(Plant plants)
        {
            _context.Plants.Add(plants);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlants", new { id = plants.Id }, plants);
        }

        // DELETE: api/PlantOverviews/5
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

        private bool PlantExists(int id)
        {
            return _context.PlantOverviews.Any(e => e.Id == id);
        }
    }
}
