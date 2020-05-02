# CSESoftware.Repository.EntityFrameworkCore

The Entity Framework Core implementation  of CSESoftware.Repository.

---

To use this, create your own DbContext, inherit from the BaseDbContext, and add your DbSets.
```
public class DbContext : BaseMockDbContext
{
 	public DbContext(DbContextOptions options) : base(options)
	{
	}

	public DbSet<Pizza> Pizzas { get; set; }
	public DbSet<Topping> Toppings { get; set; }
	public DbSet<Crust> Crusts { get; set; }
}
```

All of your entites should inherit from CSESoftware.Core.Entity<>
```
public class Pizza : BaseEntity<int>
{
	public string Name { get; set; }
	public double Cost { get; set; }

	public int CrustId { get; set; }
	public Crust Crust { get; set; }

	public int ToppingId { get; set; }
	public Topping Topping { get; set; }
}
```