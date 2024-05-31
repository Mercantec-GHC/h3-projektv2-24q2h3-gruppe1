using API.Data;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ArduinoController : ControllerBase
	{
		private readonly AppDBContext _context;

		// GET: ArduinoController
		public ArduinoController(AppDBContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Arduino>>> GetArduino()
		{
			return await _context.Arduinos.ToListAsync();
		}

		// ---------------- Plant Credentials ------------------ //

		[HttpPost("createArduino")]
		public async Task<ActionResult<Arduino>> PostPlant(CreateArduino arduino)
		{
			Arduino arduinoCreate = new Arduino
			{
				Sensor1Name = arduino.Sensor1Name,
				Sensor2Name = arduino.Sensor2Name,
				SensorId1 = arduino.SensorId1,
				SensorId2 = arduino.SensorId2,

				UpdatedAt = DateTime.UtcNow,
				CreatedAt = DateTime.UtcNow
			};

			_context.Arduinos.Add(arduinoCreate);
			await _context.SaveChangesAsync();

			return CreatedAtAction("GetArduinos", new { id = arduinoCreate.Id }, arduinoCreate);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Arduino>> GetArduino(int id)
		{
			var arduino = await _context.Arduinos.FindAsync(id);

			if (arduino == null)
			{
				return NotFound();
			}

			return arduino;
		}

		// PUT: api/PlantOverviews/id
		[HttpPut("sensorname/{id}")]
		public async Task<IActionResult> PutSettingsOverview(int id, PutSensorName settings)
		{
			if (settings == null)
			{
				return BadRequest("Settings data is null.");
			}

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var existingSettings = await _context.Arduinos.FindAsync(id);
			if (existingSettings == null)
			{
				return NotFound();
			}

			// Update properties
			existingSettings.Sensor1Name = settings.Sensor1Name;
			existingSettings.Sensor2Name = settings.Sensor2Name;

			_context.Entry(existingSettings).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!arduinoExists(id))
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

		// PUT: api/PlantOverviews/id
		[HttpPut("adduserarduino/{id}")]
		public async Task<IActionResult> PutUserIdOverview(int id, AddUserToArduino settings)
		{
			if (settings == null)
			{
				return BadRequest("User data is null.");
			}

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var existingSettings = await _context.Arduinos.FindAsync(id);
			if (existingSettings == null)
			{
				return NotFound();
			}

			// Update properties
			existingSettings.UserId = settings.UserId;

			_context.Entry(existingSettings).State = EntityState.Modified;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!arduinoExists(id))
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
		private bool arduinoExists(int id)
		{
			return _context.Arduinos.Any(e => e.Id == id);
		}

	}
}
