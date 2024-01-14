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
        [HttpPost]
        

        [Authorize(Roles = "TourOperator")]
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

            return View(tour);
        }

        [Authorize(Roles = "TourOperator")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "TourOperator")]
        public async Task<IActionResult> Create(Tour tour)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tour);
                tour.Deleted = false;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tour);
        }

        [Authorize(Roles = "TourOperator")]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "TourOperator")]
        public async Task<IActionResult> Edit(int id,Tour tour)
        {
            if (id != tour.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tour);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TourExists(tour.Id))
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
            return View(tour);
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

            return View(tour);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "TourOperator")]
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
            return RedirectToAction(nameof(Index));
        }

        private bool TourExists(int id)
        {
          return (_context.Tour?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
