using agency.Data.Migrations;
using agency.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TourAgency.Models;

namespace agency.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Tour>? Tour { get; set; }
        public DbSet<Basket>? baskets { get; set; }
        public DbSet<Order>? Orders { get; set; }
        public DbSet<OrderedTour>? OrderedTour { get; set; }
    }
}