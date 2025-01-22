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
    [Route("api/[controller]")]
    // [Authorize]
    public class OrderController : Controller
    {

        private readonly ApplicationDbContext _context;
        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetAll")]
        // [Authorize(Roles = "TourOperator")]
        public async Task<IActionResult> Index()
        {
            List<Order> orders = await _context.Orders.ToListAsync();
            foreach (Order order in orders)
            {
                order.user = await _context.Users.FindAsync(order.UserId);
            }
            return _context.Orders != null ?
                          Ok(orders) :
                          Problem("Entity set 'ApplicationDbContext.Tour'  is null.");
        }


        [HttpPost("Submit")]
        // [Authorize(Roles = "TourOperator")]
        public async Task<IActionResult> Submit([FromBody] SubmitOrderRequest request)
        {
            // Проверка данных
            if (request == null || request.Id <= 0)
            {
                return BadRequest(new { Message = "Некорректный запрос, отсутствует или неверный ID" });
            }

            // Поиск заказа
            var order = await _context.Orders.FindAsync(request.Id);
            if (order == null)
            {
                return NotFound(new { Message = "Заказ с указанным ID не найден" });
            }

            // Обновление статуса заказа
            order.OrderStatus = "Подтвержден";
            order.PaymentStatus = "Ожидает оплаты";
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Заказ успешно подтвержден", OrderId = order.Id });
        }

        [HttpPost("Cancel")]
        // [Authorize(Roles = "TourOperator")]
        public async Task<IActionResult> Cencel([FromBody] SubmitOrderRequest request)
        {

            var Order = await _context.Orders.FindAsync(request.Id);
            if (Order != null)
            {
                Order.OrderStatus = "Отменён";
                _context.Orders.Update(Order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
        {
            // Проверка входных данных
            if (request == null || string.IsNullOrEmpty(request.UserId) || request.Price <= 0)
            {
                return BadRequest(new { Message = "Некорректные данные в запросе" });
            }

            // Создание нового заказа
            var order = new Order
            {
                UserId = request.UserId,
                BuyTime = request.BuyTime,
                OrderStatus = "Ожидает подтверждения",
                Price = request.Price
            };

            _context.Add(order);
            await _context.SaveChangesAsync();

            // Получение созданного заказа
            order = _context.Orders
                .OrderBy(e => e.BuyTime)
                .LastOrDefault(w => w.UserId == request.UserId);

            if (order == null)
            {
                return StatusCode(500, new { Message = "Ошибка при создании заказа" });
            }

            // Получение элементов корзины пользователя
            var shopCartItems = _context.baskets
                .Where(m => m.UserId == request.UserId)
                .ToList();

            if (shopCartItems == null || !shopCartItems.Any())
            {
                return BadRequest(new { Message = "Корзина пользователя пуста" });
            }

            // Перенос данных из корзины в OrderedTour
            foreach (var item in shopCartItems)
            {
                var orderedTour = new OrderedTour
                {
                    OrderId = order.Id,
                    TourId = item.TourId,
                    PersonsCount = item.PersonsCount
                };

                _context.Add(orderedTour);
                _context.Remove(item);
            }

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Заказ успешно создан", OrderId = order.Id });
        }

        public async Task<IActionResult> Payment(int id, string? nomcvv, string card)
        {
            ViewBag.cvv = nomcvv; 
            ViewBag.card = card; 
            ViewBag.Id = id;
            return View();
        }
        [HttpPost("PaymentCard")]
        public async Task<IActionResult> PaymentCard([FromBody] PaymentRequest request)
        {
            string ansCVV = null;
            string ansCard = null;
            bool flagcvv = false;
            bool flagcart = false;

            try
            {
                if (int.Parse(request.CVV) < 100 || int.Parse(request.CVV) > 999 || request.CVV.Length != 3)
                {
                    flagcvv = true;
                }
            }
            catch (Exception)
            {
                flagcvv = true;
            }

            try
            {
                if (long.Parse(request.CardNum) < 1000_0000_0000_0000 || long.Parse(request.CardNum) > 9999_9999_9999_9999 || request.CardNum.Length != 16)
                {
                    flagcart = true;
                }
            }
            catch (Exception)
            {
                flagcart = true;
            }

            if (flagcvv || flagcart)
            {
                if (flagcvv)
                {
                    ansCVV = "Некорректно введенный CVV код";
                }
                if (flagcart)
                {
                    ansCard = "Некорректно введенный номер карты";
                }
                return BadRequest(new { Message = "Ошибка валидации", CVVError = ansCVV, CardError = ansCard });
            }

            var order = await _context.Orders.FindAsync(request.Id);
            if (order != null)
            {
                order.PaymentStatus = "Оплачен";
                _context.Orders.Update(order);
            }

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Оплата успешно выполнена" });
        }

        public async Task<IActionResult> Info(int id)
        {
            var order = await _context.Orders.FirstAsync(x => x.Id == id);

            return Ok(order);

            List<OrderedTour> orderedTours = _context.OrderedTour.Where(p => p.OrderId == id).ToList();

            foreach (var orderedTour in orderedTours)
            {
                orderedTour.Tour = await _context.Tour.FindAsync(orderedTour.TourId);
            }
            order.OrderedTours = orderedTours;

        }

        
    }
    public class PaymentRequest
    {
        public int Id { get; set; }
        public string CVV { get; set; }
        public string CardNum { get; set; }
    }
    public class CreateOrderRequest
    {
        public string UserId { get; set; } // ID пользователя
        public DateTime BuyTime { get; set; } // Время покупки
        public int Price { get; set; } // Цена заказа

    }
    public class SubmitOrderRequest
    {
        public int Id { get; set; } // ID заказа
    }


}
