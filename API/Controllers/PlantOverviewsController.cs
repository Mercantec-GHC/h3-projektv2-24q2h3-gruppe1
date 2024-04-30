using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Http.HttpResults;

// Planted createdTime = createdTime DateTime.UtcNow

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlantOverviewsController : ControllerBase
    {
        private readonly AppDBContext _context;

        public PlantOverviewsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/PlantOverviews
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlantOverview>>> GetPlants()
        {
            return await _context.PlantOverviews.ToListAsync();
        }

        // GET: api/PlantOverviews/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PlantOverview>> GetPlantOverview(int id)
        {
            var plantOverview = await _context.PlantOverviews.FindAsync(id);

            if (plantOverview == null)
            {
                return NotFound();
            }

            return plantOverview;
        }

        // PUT: api/PlantOverviews/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlantOverview(int id, PlantOverview plantOverview)
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
                if (!PlantOverviewExists(id))
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
        public async Task<ActionResult<PlantOverview>> PostPlantOverview(PlantOverview plantOverview)
        {
               // is not inf
            plantOverview.UpdatedAt = DateTime.UtcNow;
            plantOverview.CreatedAt = DateTime.UtcNow;
            _context.PlantOverviews.Add(plantOverview);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlantOverview", new { id = plantOverview.Id }, plantOverview);
        }

        // DELETE: api/PlantOverviews/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlantOverview(int id)
        {
            var plantOverview = await _context.PlantOverviews.FindAsync(id);
            if (plantOverview == null)
            {
                return NotFound();
            }

            _context.PlantOverviews.Remove(plantOverview);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlantOverviewExists(int id)
        {
            return _context.PlantOverviews.Any(e => e.Id == id);
        }
    }
}
