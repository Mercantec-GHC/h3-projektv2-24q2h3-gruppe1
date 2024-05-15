using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Models;

// Planted createdTime = createdTime DateTime.UtcNow

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController : ControllerBase
    {
        private readonly AppDBContext _context;

        public SettingsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/PlantOverviews
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Setting>>> GetSettings()
        {
            return await _context.Setting.ToListAsync();
        }

        // GET: api/PlantOverviews/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Setting>> GetPlantOverview(int id)
        {
            var settings = await _context.Setting.FindAsync(id);

            if (settings == null)
            {
                return NotFound();
            }

            return settings;
        }

        // PUT: api/PlantOverviews/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlantOverview(int id, Setting settings)
        {
            if (id != settings.Id)
            {
                return BadRequest();
            }

            _context.Entry(settings).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SettingExists(id))
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
        public async Task<ActionResult<Setting>> PostSettings(Setting settings)
        {
            // is not inf
            settings.UpdatedAt = DateTime.UtcNow;
            settings.CreatedAt = DateTime.UtcNow;
            _context.Setting.Add(settings);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlantOverview", new { id = settings.Id }, settings);
        }

        // DELETE: api/PlantOverviews/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSetting(int id)
        {
            var settings = await _context.Setting.FindAsync(id);
            if (settings == null)
            {
                return NotFound();
            }

            _context.Setting.Remove(settings);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SettingExists(int id)
        {
            return _context.Setting.Any(e => e.Id == id);
        }
    }
}
