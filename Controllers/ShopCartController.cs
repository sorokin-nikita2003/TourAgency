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
    [Route("api/[controller]")]
    //[Authorize]
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

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] AddBasketRequest request)
        {
            // Проверка валидности данных
            if (request == null || request.Id <= 0 || string.IsNullOrEmpty(request.UserId) || request.PersonsCount <= 0)
            {
                return BadRequest(new { Message = "Некорректные данные в запросе" });
            }

            // Проверка наличия записи в корзине
            var basketTour = await _context.baskets
                .FirstOrDefaultAsync(m => m.TourId == request.Id && m.UserId == request.UserId);

            if (basketTour == null)
            {
                // Создание новой записи в корзине
                basketTour = new Basket
                {
                    TourId = request.Id,
                    UserId = request.UserId,
                    PersonsCount = request.PersonsCount
                };
                _context.Add(basketTour);
            }
            else
            {
                // Обновление существующей записи в корзине
                basketTour.PersonsCount += request.PersonsCount;
                _context.Update(basketTour);
            }

            await _context.SaveChangesAsync();

            // Возврат успешного ответа
            return Ok(new { Message = "Тур успешно добавлен в корзину" });
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
    public class AddBasketRequest
    {
        public int Id { get; set; } // ID тура
        public string UserId { get; set; } // ID пользователя
        public int PersonsCount { get; set; } // Количество человек
    }

}
