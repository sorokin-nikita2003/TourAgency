using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using TourAgency.Models;

namespace agency.Models
{
    public class OrderedTour
    {
        public int Id { get; set; }
        public int PersonsCount { get; set; }
        public int? TourId { get; set; }
        public int? OrderId { get; set; }
        // Keys
        public Tour? Tour { get; set; }
        public Order? order { get; set; }
    }
}
