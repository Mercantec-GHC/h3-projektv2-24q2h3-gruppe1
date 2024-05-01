using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Models;

namespace API.Controllers
{
    public class PlantOverviewsController : Controller
    {
        private readonly AppDBContext _context;

        public PlantOverviewsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: PlantOverviews
        public async Task<IActionResult> Index()
        {
            return View(await _context.PlantOverviews.ToListAsync());
        }

        // GET: PlantOverviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plantOverview = await _context.PlantOverviews
                .FirstOrDefaultAsync(m => m.Id == id);
            if (plantOverview == null)
            {
                return NotFound();
            }

            return View(plantOverview);
        }

        // GET: PlantOverviews/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PlantOverviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PlantNameId,MoistureLevel,MinWaterLevel,MaxWaterLevel,sensorId,Id,CreatedAt,UpdatedAt")] PlantOverview plantOverview)
        {
            if (ModelState.IsValid)
            {
                _context.Add(plantOverview);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(plantOverview);
        }

        // GET: PlantOverviews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plantOverview = await _context.PlantOverviews.FindAsync(id);
            if (plantOverview == null)
            {
                return NotFound();
            }
            return View(plantOverview);
        }

        // POST: PlantOverviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PlantNameId,MoistureLevel,MinWaterLevel,MaxWaterLevel,sensorId,Id,CreatedAt,UpdatedAt")] PlantOverview plantOverview)
        {
            if (id != plantOverview.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(plantOverview);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlantOverviewExists(plantOverview.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(plantOverview);
        }

        // GET: PlantOverviews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plantOverview = await _context.PlantOverviews
                .FirstOrDefaultAsync(m => m.Id == id);
            if (plantOverview == null)
            {
                return NotFound();
            }

            return View(plantOverview);
        }

        // POST: PlantOverviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var plantOverview = await _context.PlantOverviews.FindAsync(id);
            if (plantOverview != null)
            {
                _context.PlantOverviews.Remove(plantOverview);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlantOverviewExists(int id)
        {
            return _context.PlantOverviews.Any(e => e.Id == id);
        }
    }
}
