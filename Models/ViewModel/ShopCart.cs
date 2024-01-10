using agency.Models;

namespace TourAgency.Models.ViewModel
{
    public class ShopCart
    {
        public List<Tour> tours { get; set; }
        public List<Basket> basket { get; set; }

        public int price { get; set; }
        public int priceHotTour { get; set; }

        public ShopCart() 
        {
            price = 0;
            priceHotTour = 0;
        }
    }
}
