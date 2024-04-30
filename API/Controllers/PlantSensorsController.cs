using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Models;
using System.Numerics;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlantSensorsController : ControllerBase
    {
        private readonly AppDBContext _context;

        public PlantSensorsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/PlantSensors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlantSensor>>> GetPlantSensor()
        {
            return await _context.PlantSensor.ToListAsync();
        }

        // GET: api/PlantSensors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PlantSensor>> GetPlantSensor(int id)
        {
            var plantSensor = await _context.PlantSensor.FindAsync(id);

            if (plantSensor == null)
            {
                return NotFound();
            }

            return plantSensor;
        }

        // PUT: api/PlantSensors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlantSensor(int id, PlantSensor plantSensor)
        {
            if (id != plantSensor.Id)
            {
                return BadRequest();
            }

            _context.Entry(plantSensor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlantSensorExists(id))
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

        // POST: api/PlantSensors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PlantSensor>> PostPlantSensor(PlantSensor plantSensor)
        {
            // is not inf
            plantSensor.UpdatedAt = DateTime.UtcNow;
            plantSensor.CreatedAt = DateTime.UtcNow;
            _context.PlantSensor.Add(plantSensor);
            await _context.SaveChangesAsync();  

            return CreatedAtAction("GetPlantSensor", new { id = plantSensor.Id }, plantSensor);
        }

        // DELETE: api/PlantSensors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlantSensor(int id)
        {
            var plantSensor = await _context.PlantSensor.FindAsync(id);
            if (plantSensor == null)
            {
                return NotFound();
            }

            _context.PlantSensor.Remove(plantSensor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlantSensorExists(int id)
        {
            return _context.PlantSensor.Any(e => e.Id == id);
        }
    }
}
