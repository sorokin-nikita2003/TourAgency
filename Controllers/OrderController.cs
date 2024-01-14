using agency.Data;
using agency.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using static NuGet.Packaging.PackagingConstants;

namespace agency.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {

        
        private readonly ApplicationDbContext _context;
        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "TourOperator")]
        public async Task<IActionResult> Index()
        {
            List<Order> orders = await _context.Orders.ToListAsync();
            foreach (Order order in orders)
            {
                order.user = await _context.Users.FindAsync(order.UserId);
            }
            return _context.Orders != null ?
                          View(orders) :
                          Problem("Entity set 'ApplicationDbContext.Tour'  is null.");
        }


        [Authorize(Roles = "TourOperator")]
        public async Task<IActionResult> Submit(int id)
        {

            var Order = await _context.Orders.FindAsync(id);
            if (Order != null)
            {
                Order.OrderStatus = "Подтвержден";
                Order.PaymentStatus = "Ожидает оплаты";
                _context.Orders.Update(Order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        [Authorize(Roles = "TourOperator")]
        public async Task<IActionResult> Cencel(int id)
        {

            var Order = await _context.Orders.FindAsync(id);
            if (Order != null)
            {
                Order.OrderStatus = "Отменён";
                _context.Orders.Update(Order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Create(int price) 
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Order order = new Order();
            order.UserId = userId;
            order.BuyTime = DateTime.Now;
            order.OrderStatus = "Ожидает подтверждения";
            order.Price = price;

            _context.Add(order);
            _context.SaveChanges();
            order = _context.Orders.OrderBy(e => e.BuyTime).LastOrDefault(w => w.UserId == userId);

            List<Basket> shopCartItems = _context.baskets.Where(m => m.UserId == userId).ToList();
            OrderedTour orderedTour;


            foreach (var item in shopCartItems)
            {
                orderedTour = new OrderedTour();
                orderedTour.OrderId = order.Id;
                orderedTour.TourId = item.TourId;
                orderedTour.PersonsCount = item.PersonsCount;
                _context.Add(orderedTour);
                _context.Remove(item);
            }
            _context.SaveChanges();

            return RedirectToAction("Index", "Tours");
        }
        public async Task<IActionResult> Payment(int id, string? nomcvv, string card)
        {
            ViewBag.cvv = nomcvv; 
            ViewBag.card = card; 
            ViewBag.Id = id;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> PaymentCard(int id, string cvv, string cartNum)
        {
            string ansCVV = null;
            string ansCard = null;
            bool flagcvv = false;
            bool flagcart = false;
            try
            {
                if(int.Parse(cvv) < 100 || int.Parse(cvv) > 999 || cvv.Length != 3)
                {
                    flagcvv = true;
                }
            } catch (Exception ex)
            {
                flagcvv = true;
            }
            try
            {
                if (long.Parse(cartNum) < 1000_0000_0000_0000 || long.Parse(cartNum) > 9999_9999_9999_9999 || cartNum.Length != 16)
                {
                    flagcart = true;
                }
            }
            catch (Exception ex)
            {
                flagcart = true;
            }

            if (flagcvv || flagcart) 
            {
                if(flagcvv)
                {
                    ansCVV = "Некорректно введенный cvv код";
                }
                if (flagcart)
                {
                    ansCard = "Некорректно введенный номер карты";
                }
                return RedirectToAction("Payment", new { id = id, nomcvv = ansCVV, card = ansCard });
            }



            var Order = await _context.Orders.FindAsync(id);
            if (Order != null)
            {
                Order.PaymentStatus = "Оплачен";
                _context.Orders.Update(Order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("MyOrders", "Profile");
        }
        public async Task<IActionResult> Info(int id)
        {
            var order = await _context.Orders.FirstAsync(x => x.Id == id);

            List<OrderedTour> orderedTours = _context.OrderedTour.Where(p => p.OrderId == id).ToList();

            foreach (var orderedTour in orderedTours)
            {
                orderedTour.Tour = await _context.Tour.FindAsync(orderedTour.TourId);
            }
            order.OrderedTours = orderedTours;

            return View(order);
        }

        
    }
}
