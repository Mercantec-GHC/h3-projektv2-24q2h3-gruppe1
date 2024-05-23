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

        // ---------------- Settings info ---------------------- //


        // GET: api/PlantOverviews
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Setting>>> GetSettings()
        {
            return await _context.Setting.ToListAsync();
        }


        // POST: api/PlantOverviews
        [HttpPost]
        public async Task<ActionResult<Setting>> PostSettings(Setting settings)
        {
            // is not inf
            settings.UpdatedAt = DateTime.UtcNow;
            settings.CreatedAt = DateTime.UtcNow;
            _context.Setting.Add(settings);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSettingsOverview", new { id = settings.Id }, settings);
        }

        // ----------------------- ID -------------------------- //


        // GET: api/PlantOverviews/id
        [HttpGet("{id}")]
        public async Task<ActionResult<Setting>> GetSettingsOverview(int id)
        {
            var settings = await _context.Setting.FindAsync(id);

            if (settings == null)
            {
                return NotFound();
            }

            return settings;
        }

        // PUT: api/PlantOverviews/id
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSettingsOverview(int id, Setting settings)
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

        // DELETE: api/PlantOverviews/id
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

        // ----------------------- Mis ------------------------ //

        private bool SettingExists(int id)
        {
            return _context.Setting.Any(e => e.Id == id);
        }
    }
}
