using Microsoft.EntityFrameworkCore;

namespace CSESoftware.Repository.EntityFrameworkCore.TestProject.Setup
{
    public class DbContext : BaseDbContext
    {
        public DbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Person> Persons { get; set; }
        public DbSet<PersonPizza> PersonPizzas { get; set; }
        public DbSet<Pizza> Pizzas { get; set; }
        public DbSet<Topping> Toppings { get; set; }
        public DbSet<Crust> Crusts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PersonPizza>()
                .HasKey(pp => new { pp.PersonId, pp.PizzaId });

            modelBuilder.Entity<PersonPizza>()
                .HasOne(pp => pp.Person)
                .WithMany(p => p.PersonPizzas)
                .HasForeignKey(pp => pp.PersonId);

            modelBuilder.Entity<PersonPizza>()
                .HasOne(pp => pp.Pizza)
                .WithMany(p => p.PersonPizzas)
                .HasForeignKey(pp => pp.PizzaId);
        }
    }
}
