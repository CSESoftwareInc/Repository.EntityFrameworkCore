using Microsoft.EntityFrameworkCore;

namespace CSESoftware.Repository.EntityFrameworkCore.TestProject.Setup
{
    public class DbContext : BaseDbContext
    {
        public DbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Pizza> Pizzas { get; set; }
        public DbSet<Topping> Toppings { get; set; }
        public DbSet<Crust> Crusts { get; set; }
    }
}