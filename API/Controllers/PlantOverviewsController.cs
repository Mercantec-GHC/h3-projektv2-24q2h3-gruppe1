using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Models;

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

        // ---------------- Plant Credentials ------------------ //

        // POST: api/PlantOverviews
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
        // POST: api/PlantOverviews
        [HttpPost("postValue")]
        public async Task<ActionResult<PlantOverview>> PostValue(PostMoistureValue postValue)
        {
            PlantOverview plantOverview = new PlantOverview()
            {
                MoistureLevel = postValue.MoistureLevel,
                sensorId = postValue.sensorId,
                ArduinoId = postValue.ArduinoId,
                PlantName = postValue.PlantName,
                UpdatedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
            _context.PlantOverviews.Add(plantOverview);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlantOverview", new { id = plantOverview.Id }, plantOverview);
        }
        // ----------------------- ID -------------------------- //

        // GET: api/PlantOverviews/id
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

		// PUT: api/PlantOverviews/id
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

        // ----------------------- Mis ------------------------ //

        private bool PlantOverviewExists(int id)
		{
			return _context.PlantOverviews.Any(e => e.Id == id);
		}
	}
}
