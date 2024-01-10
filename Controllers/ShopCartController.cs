using agency.Data;
using agency.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TourAgency.Models;
using TourAgency.Models.ViewModel;

namespace agency.Controllers
{
    [Authorize]
    public class ShopCartController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ShopCartController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<Basket> shopCart = _context.baskets.Where(m => m.UserId == userId).ToList();


            ShopCart sc = new ShopCart();

            List<Tour> tourList = new List<Tour>();

            
            foreach (var basket in shopCart)
            {
                var tour = await _context.Tour.FirstOrDefaultAsync(m => m.Id == basket.TourId);
                sc.price += tour.Price * basket.PersonsCount;
                if(tour.HotTour)
                {
                    sc.priceHotTour += (int)(tour.Price * basket.PersonsCount * 0.8);
                } else
                {
                    sc.priceHotTour += tour.Price * basket.PersonsCount;
                }
                tourList.Add(tour);
            }


            sc.tours = tourList;
            sc.basket = shopCart;

            return View(sc);
        }
        public async Task<IActionResult> Add(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            Basket basketTour = await _context.baskets
                .FirstOrDefaultAsync(m => m.TourId == id && m.UserId == userId);

            if (basketTour == null)
            {
                basketTour = new Basket();
                basketTour.TourId = id;
                basketTour.UserId = userId;
                basketTour.PersonsCount = 1;
                _context.Add(basketTour);
                _context.SaveChanges();
            }
            else
            {
                basketTour.PersonsCount++;
                _context.Update(basketTour);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {

            var basketTour = await _context.baskets.FindAsync(id);
            if (basketTour != null)
            {
                _context.baskets.Remove(basketTour);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> PlusPerson(int id)
        {

            var basketTour = await _context.baskets.FindAsync(id);
            if (basketTour != null)
            {
                basketTour.PersonsCount++;
                _context.baskets.Update(basketTour);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> MinusPerson(int id)
        {

            var basketTour = await _context.baskets.FindAsync(id);
            if (basketTour != null)
            {
                basketTour.PersonsCount--;
                _context.baskets.Update(basketTour);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }




    }
}
