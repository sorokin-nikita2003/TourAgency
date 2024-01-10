using agency.Models;
using System.Globalization;

namespace TourAgency.Models
{
    // Туры
    public class Tour
    {
        public int Id { get; set; }
        public int Price { get; set; }
        public string? TourName { get; set; }
        public string? Country { get; set; }
        public string? Description { get; set; }
        public string? Info { get; set; }
        public bool HotTour { get; set; }
        public bool Deleted { get; set; }
        public DateTime DateStart { get; set; }

        //Keys

        public List<Basket>? Baskets { get; set; }
        public List<OrderedTour>? OrderedTours { get; set; }



    }
}
