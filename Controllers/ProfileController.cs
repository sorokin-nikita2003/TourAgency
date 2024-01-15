using agency.Data;
using agency.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Security.Claims;

namespace agency.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            IdentityUser user = await _context.Users.FindAsync(userId);

            return View(user);
        }
        //MyOrders
        public IActionResult MyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<Order> orders = _context.Orders.Where(p => p.UserId == userId).ToList();

            return View(orders);
        }

        public IActionResult Cencel(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Order order = _context.Orders.Find(id);

            order.OrderStatus = "Отменён";

            _context.Orders.Update(order);
            _context.SaveChanges();

            return RedirectToAction(nameof(MyOrders));
        }
        [Authorize(Roles = "TourOperator")]
        public async Task<IActionResult> Stat() 
        {
            Dictionary<string, int> countrys = new Dictionary<string, int>();
            var ot = await _context.OrderedTour.ToListAsync();

            foreach(var country in ot)
            {
                country.Tour = await _context.Tour.FindAsync(country.TourId);
            }

            foreach(var i in ot)
            {
                if(countrys.Keys.Contains(i.Tour.Country))
                {
                    countrys[i.Tour.Country] += i.PersonsCount;
                } else
                {
                    countrys[i.Tour.Country] = i.PersonsCount;
                }
            }

            return View(countrys);
        }
    }
}
