using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TourAgency.Models;
using agency.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using agency.Models;
using Microsoft.AspNetCore.Authorization;

namespace agency.Controllers
{
    [Route("api/[controller]")]
    public class ToursController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ToursController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return _context.Tour != null ?
                          View(await _context.Tour.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Tour'  is null.");
        }
        [HttpGet("GetAll")]
        public IActionResult Search(bool hotTour, string searchString)
        {
            List<Tour> Tours = _context.Tour.Where(x => !x.Deleted).ToList();
            return Ok(Tours);

            List<Tour> filteredTours = null;

            if (!String.IsNullOrEmpty(searchString))
            {
                filteredTours = Tours.Where(s => s.TourName.ToLower()!.Contains(searchString.ToLower())).ToList();
                Tours = filteredTours;
            }
            if (hotTour != false && hotTour != null)
            {
                filteredTours = Tours.Where(s => s.HotTour == true).ToList();
                Tours = filteredTours;
            }
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Tour == null)
            {
                return NotFound();
            }

            var tour = await _context.Tour
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tour == null)
            {
                return NotFound();
            }

            return Ok(tour);
        }

        [Authorize(Roles = "TourOperator")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("Create")]
        // [ValidateAntiForgeryToken]
        // [Authorize(Roles = "TourOperator")]
        public async Task<IActionResult> CreateTour([FromBody] Tour tour)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Добавляем тур в базу данных
            _context.Tour.Add(tour);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Details), new { id = tour.Id }, tour); // Возвращаем созданный объект
        }

        // [Authorize(Roles = "TourOperator")]
        public async Task<IActionResult> Edit(int? id)
        {
            // will give the user's userId

            if (id == null || _context.Tour == null)
            {
                return NotFound();
            }

            var tour = await _context.Tour.FindAsync(id);

            if (tour == null)
            {
                return NotFound();
            }
            return View(tour);
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit([FromBody] Tour tour)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                            .Select(e => e.ErrorMessage);
                return BadRequest(new { Message = "Invalid model", Errors = errors });
            }

            try
            {
                if (!TourExists(tour.Id))
                {
                    return NotFound(new { Message = "Tour not found" });
                }

                _context.Attach(tour).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Tour updated successfully" });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine($"Concurrency error: {ex.Message}");
                return StatusCode(500, new { Message = "Database concurrency error" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, new { Message = "An unexpected error occurred" });
            }
        }


        [Authorize(Roles = "TourOperator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Tour == null)
            {
                return NotFound();
            }

            var tour = await _context.Tour
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tour == null)
            {
                return NotFound();
            }

            return Ok(tour);
        }

        [HttpPost("Delete/{id}")]
        // [ValidateAntiForgeryToken]
        // [Authorize(Roles = "TourOperator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Tour == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Tour'  is null.");
            }
            var tour = await _context.Tour.FindAsync(id);
            if (tour != null)
            {
                tour.Deleted = true;
                _context.Tour.Update(tour);
            }
            
            await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
            return Ok(id);
        }

        private bool TourExists(int id)
        {
          return (_context.Tour?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }

    public class TourDto
    {
        public string TourName { get; set; }
        public string Country { get; set; }
        public decimal Price { get; set; }
        public string DateStart { get; set; }
        public string Description { get; set; }
        public bool HotTour { get; set; }
    }
}
