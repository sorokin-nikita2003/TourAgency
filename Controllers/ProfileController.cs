using agency.Data;
using agency.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Providers.Entities;

namespace agency.Controllers
{
    [Route("api/[controller]")]
    // [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager; // Добавим UserManager для регистрации
        private readonly SignInManager<IdentityUser> _signInManager;


        public ProfileController(ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager; // Инициализируем UserManager
            _signInManager = signInManager;
        }

        // Метод для регистрации клиента
        [HttpPost("RegisterClient")]
        public async Task<IActionResult> RegisterClient([FromBody] RegisterClientModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser { Email = model.Email, UserName = model.Email };
                // добавляем пользователя
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // установка куки
                    await _signInManager.SignInAsync(user, false);
                    await _userManager.AddToRoleAsync(user, "BUYER");
                    return Ok(model);
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            return BadRequest("Некорректные данные");
        }

        // Модель для регистрации покупателя
        //public class RegisterClientModel
        //{
        //    public string UserName { get; set; }
        //    public string Password { get; set; }
        //}

        // Пример других методов контроллера

        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            IdentityUser user = await _context.Users.FindAsync(userId);

            return View(user);
        }

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

        [HttpGet("Stat")]
        // [Authorize(Roles = "TourOperator")]
        public async Task<IActionResult> Stat()
        {
            Dictionary<string, int> countrys = new Dictionary<string, int>();
            var ot = await _context.OrderedTour.ToListAsync();

            foreach (var country in ot)
            {
                country.Tour = await _context.Tour.FindAsync(country.TourId);
            }

            foreach (var i in ot)
            {
                if (countrys.Keys.Contains(i.Tour.Country))
                {
                    countrys[i.Tour.Country] += i.PersonsCount;
                }
                else
                {
                    countrys[i.Tour.Country] = i.PersonsCount;
                }
            }

            return Ok(countrys);
        }
    }
}
