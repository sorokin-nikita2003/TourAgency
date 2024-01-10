using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using TourAgency.Models;

namespace agency.Models
{
    // Заказы
    public class Order
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public DateTime BuyTime { get; set; }
        public string OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public int Price { get; set; }

        // keys
        public IdentityUser? user { get; set; }
        public List<OrderedTour>? OrderedTours { get; set; }
    }
}
