using CSESoftware.Repository.Builder;
using CSESoftware.Repository.EntityFrameworkCore.TestProject.Setup;
using Microsoft.EntityFrameworkCore;
using DbContext = CSESoftware.Repository.EntityFrameworkCore.TestProject.Setup.DbContext;

namespace CSESoftware.Repository.EntityFrameworkCore.TestProject
{
    public class ReadOnlyRepositoryTests
    {
        #region Initial Data

        public static List<Topping> Toppings = new List<Topping>
        {
            new Topping
            {
                Id = 1,
                Name = "Bacon",
                AdditionalCost = 0.5,
            },
            new Topping
            {
                Id = 2,
                Name = "Pepperoni",
                AdditionalCost = 0,
            },
            new Topping
            {
                Id = 3,
                Name = "Sausage",
                AdditionalCost = 0,
            },
            new Topping
            {
                Id = 4,
                Name = "Pineapple",
                AdditionalCost = 0,
            }
        };

        public static List<Crust> Crusts = new List<Crust>
        {
            new Crust
            {
                Id = 1,
                Name = "Pan Crust",
                AdditionalCost = 1,
            },
            new Crust
            {
                Id = 2,
                Name = "Thin Crust",
                AdditionalCost = 0,
            },
            new Crust
            {
                Id = 3,
                Name = "Regular Crust",
                AdditionalCost = 0,
            }
        };

        public static List<Pizza> Pizzas = new List<Pizza>
        {
            new Pizza
            {
                Id = 1,
                Name = "Pan Sausage",
                Cost = 10,
                CrustId = 1,
                ToppingId = 3,
            },
            new Pizza
            {
                Id = 2,
                Name = "Thin Peperoni",
                Cost = 8,
                CrustId = 2,
                ToppingId = 2,
            },
            new Pizza
            {
                Id = 3,
                Name = "Hawaiian",
                Cost = 12,
                CrustId = 3,
                ToppingId = 4,
            }
        };

        public static List<Person> People = new List<Person>
        {
            new Person
            {
                Id = 1,
                Name = "Cameron",
                PersonPizzas = new List<PersonPizza>
                {
                    new PersonPizza
                    {
                        PersonId = 1,
                        PizzaId = 1
                    }
                }
            },
            new Person
            {
                Id = 2,
                Name = "James",
                PersonPizzas = new List<PersonPizza>
                {
                    new PersonPizza
                    {
                        PersonId = 2,
                        PizzaId = 3
                    }
                }
            }
        };

        public static List<Person> DuplicatePeople = new List<Person>
        {
            new Person
            {
                Id = 1,
                Name = "Name"
            },
            new Person
            {
                Id = 2,
                Name = "Name"
            },
            new Person
            {
                Id = 3,
                Name = "Name"
            },
            new Person
            {
                Id = 4,
                Name = "Name"
            }
        };

        #endregion

        #region Repository Setup

        private static DbContextOptions<DbContext> GetOptions()
        {
            return new DbContextOptionsBuilder<DbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        }

        private static ReadOnlyRepository<DbContext> GetReadOnlyRepository(DbContextOptions options)
        {
            return new ReadOnlyRepository<DbContext>(new DbContext(options));
        }

        private static async Task AddDefaultMenuItems(DbContextOptions options)
        {
            var repo = new Repository<DbContext>(new DbContext(options));

            repo.Create(Crusts);
            repo.Create(Toppings);
            repo.Create(Pizzas);
            repo.Create(People);
            await repo.SaveAsync();
        }

        private static async Task AddDuplicateMenuItems(DbContextOptions options)
        {
            var repo = new Repository<DbContext>(new DbContext(options));

            repo.Create(DuplicatePeople);
            await repo.SaveAsync();
        }

        #endregion

        [Fact]
        public async Task GetAllDistinctNamesAsyncTest()
        {
            var options = GetOptions();
            var repository = GetReadOnlyRepository(options);
            await AddDuplicateMenuItems(options);

            var query = new QueryBuilder<Person>()
                .Select(x => x.Name)
                .Distinct()
                .Build();

            var names = await repository.GetAllWithSelectAsync(query);

            Assert.Equal("Name", names.First());
        }

        [Fact]
        public async Task GetDistinctCountIdsAsyncTest()
        {
            var options = GetOptions();
            var repository = GetReadOnlyRepository(options);
            await AddDuplicateMenuItems(options);

            var query = new QueryBuilder<Person>()
                .Select(x => x.Id)
                .Distinct()
                .Build();

            var count = await repository.GetCountWithSelectAsync(query);

            Assert.Equal(4, count);
        }

        [Fact]
        public async Task GetDistinctCountNamesAsyncTest()
        {
            var options = GetOptions();
            var repository = GetReadOnlyRepository(options);
            await AddDuplicateMenuItems(options);

            var query = new QueryBuilder<Person>()
                .Select(x => x.Name)
                .Distinct()
                .Build();

            var count = await repository.GetCountWithSelectAsync(query);

            Assert.Equal(1, count);
        }

        [Fact]
        public async Task GetAllAsyncTest()
        {
            var options = GetOptions();
            var repository = GetReadOnlyRepository(options);
            await AddDefaultMenuItems(options);

            var allToppings = await repository.GetAllAsync<Topping>();
            Assert.Equal(4, allToppings.Count);

            var extraCostToppings = await repository.GetAllAsync<Topping>(x => x.AdditionalCost > 0);
            Assert.Equal(1, extraCostToppings.Count);

            // Where
            var freeToppings = new QueryBuilder<Topping>().Where(x => x.AdditionalCost.Equals(0)).Build();
            var inactiveToppings = await repository.GetAllAsync(freeToppings);
            Assert.Equal(3, inactiveToppings.Count);

            // OrderBy
            var pizzasOrderedByCostQuery = new QueryBuilder<Pizza>().OrderBy(x => x.OrderBy(y => y.Cost)).Build();
            var pizzasOrderedByCost = await repository.GetAllAsync(pizzasOrderedByCostQuery);
            Assert.Equal(8, pizzasOrderedByCost[0].Cost);
            Assert.Equal(10, pizzasOrderedByCost[1].Cost);
            Assert.Equal(12, pizzasOrderedByCost[2].Cost);

            // Include
            var repository2 = GetReadOnlyRepository(options);
            var includeQuery = new QueryBuilder<Pizza>().Include(x => x.Topping).Build();
            var pizzasWithTopping = await repository2.GetAllAsync(includeQuery);

            Assert.NotNull(pizzasWithTopping.FirstOrDefault()?.Topping);
            Assert.Null(pizzasWithTopping.FirstOrDefault()?.Crust);

            // Skip
            var skipToppingsQuery = new QueryBuilder<Topping>().Skip(2).Build();
            var toppingsSkip2 = await repository.GetAllAsync(skipToppingsQuery);
            Assert.Equal(3, toppingsSkip2.FirstOrDefault()?.Id);
            Assert.Equal(2, toppingsSkip2.Count);

            // Take
            var takeToppingsQuery = new QueryBuilder<Topping>().Take(2).Build();
            var toppingsTake2 = await repository.GetAllAsync(takeToppingsQuery);
            Assert.Equal(1, toppingsTake2.FirstOrDefault()?.Id);
            Assert.Equal(2, toppingsTake2.Count);
        }

        [Fact]
        public async Task GetAllTest()
        {
            var options = GetOptions();
            var repository = GetReadOnlyRepository(options);
            await AddDefaultMenuItems(options);

            var allToppings = repository.GetAll<Topping>();
            Assert.Equal(4, allToppings.Count);

            var extraCostToppings = repository.GetAll<Topping>(x => x.AdditionalCost > 0);
            Assert.Equal(1, extraCostToppings.Count);

            // Where
            var freeToppings = new QueryBuilder<Topping>().Where(x => x.AdditionalCost.Equals(0)).Build();
            var inactiveToppings = repository.GetAll(freeToppings);
            Assert.Equal(3, inactiveToppings.Count);

            // OrderBy
            var pizzasOrderedByCostQuery = new QueryBuilder<Pizza>().OrderBy(x => x.OrderBy(y => y.Cost)).Build();
            var pizzasOrderedByCost = repository.GetAll(pizzasOrderedByCostQuery);
            Assert.Equal(8, pizzasOrderedByCost[0].Cost);
            Assert.Equal(10, pizzasOrderedByCost[1].Cost);
            Assert.Equal(12, pizzasOrderedByCost[2].Cost);

            // Include
            var repository2 = GetReadOnlyRepository(options);
            var includeQuery = new QueryBuilder<Pizza>().Include(x => x.Topping).Build();
            var pizzasWithTopping = repository2.GetAll(includeQuery);

            Assert.NotNull(pizzasWithTopping.FirstOrDefault()?.Topping);
            Assert.Null(pizzasWithTopping.FirstOrDefault()?.Crust);

            // Skip
            var skipToppingsQuery = new QueryBuilder<Topping>().Skip(2).Build();
            var toppingsSkip2 = repository.GetAll(skipToppingsQuery);
            Assert.Equal(3, toppingsSkip2.FirstOrDefault()?.Id);
            Assert.Equal(2, toppingsSkip2.Count);

            // Take
            var takeToppingsQuery = new QueryBuilder<Topping>().Take(2).Build();
            var toppingsTake2 = repository.GetAll(takeToppingsQuery);
            Assert.Equal(1, toppingsTake2.FirstOrDefault()?.Id);
            Assert.Equal(2, toppingsTake2.Count);
        }

        [Fact]
        public async Task GetAllWithSelectAsyncTest()
        {
            var options = GetOptions();
            var repository = GetReadOnlyRepository(options);
            await AddDefaultMenuItems(options);

            var toppings = await repository.GetAllWithSelectAsync(
                new QueryBuilder<Topping>()
                    .Select(x => x.Name)
                    .Build());

            var firstTopping = toppings.FirstOrDefault();
            Assert.Equal("Bacon", firstTopping);
        }

        [Fact]
        public async Task GetAllWithSelectIdsTest()
        {
            var options = GetOptions();
            var repository = GetReadOnlyRepository(options);
            await AddDefaultMenuItems(options);

            var toppings = await repository.GetAllWithSelectAsync(
                new QueryBuilder<Topping>()
                    .Select(x => x.Id)
                    .Build());

            var firstToppingId = toppings.FirstOrDefault();
            Assert.Equal(1, firstToppingId);
        }

        [Fact]
        public async Task GetFirstAsyncQueryTest()
        {
            var options = GetOptions();
            var repository = GetReadOnlyRepository(options);
            await AddDefaultMenuItems(options);

            var result = await repository.GetFirstAsync(new QueryBuilder<Pizza>().Where(x => x.CrustId == 3).Build());

            Assert.Equal("Hawaiian", result.Name);
        }

        [Fact]
        public async Task GetFirstQueryTest()
        {
            var options = GetOptions();
            var repository = GetReadOnlyRepository(options);
            await AddDefaultMenuItems(options);

            var result = repository.GetFirst(new QueryBuilder<Pizza>().Where(x => x.CrustId == 3).Build());

            Assert.Equal("Hawaiian", result.Name);
        }

        [Fact]
        public async Task GetFirstAsyncFilterTest()
        {
            var options = GetOptions();
            var repository = GetReadOnlyRepository(options);
            await AddDefaultMenuItems(options);

            var result = await repository.GetFirstAsync<Pizza>(x => x.CrustId == 3);

            Assert.Equal("Hawaiian", result.Name);
        }

        [Fact]
        public async Task GetFirstFilterTest()
        {
            var options = GetOptions();
            var repository = GetReadOnlyRepository(options);
            await AddDefaultMenuItems(options);

            var result = repository.GetFirst<Pizza>(x => x.CrustId == 3);

            Assert.Equal("Hawaiian", result.Name);
        }

        [Fact]
        public async Task GetCountAsyncQueryTest()
        {
            var options = GetOptions();
            var repository = GetReadOnlyRepository(options);
            await AddDefaultMenuItems(options);

            var result1 = await repository.GetCountAsync(new QueryBuilder<Topping>().Where(x => x.Id == 1).Build());
            var result2 = await repository.GetCountAsync(new QueryBuilder<Topping>().Where(x => x.AdditionalCost > 0).Build());

            Assert.Equal(1, result1);
            Assert.Equal(1, result2);
        }

        [Fact]
        public async Task GetCountAsyncFilterTest()
        {
            var options = GetOptions();
            var repository = GetReadOnlyRepository(options);
            await AddDefaultMenuItems(options);

            var result1 = await repository.GetCountAsync<Topping>();
            var result2 = await repository.GetCountAsync<Topping>(x => x.Id == 1);
            var result3 = await repository.GetCountAsync<Topping>(x => x.AdditionalCost > 0);

            Assert.Equal(4, result1);
            Assert.Equal(1, result2);
            Assert.Equal(1, result3);
        }

        [Fact]
        public async Task GetExistsAsyncQueryTest()
        {
            var options = GetOptions();
            var repository = GetReadOnlyRepository(options);
            await AddDefaultMenuItems(options);

            var result1 = await repository.GetExistsAsync(new QueryBuilder<Topping>().Where(x => x.Name.Equals("Bacon")).Build());
            var result2 = await repository.GetExistsAsync(new QueryBuilder<Topping>().Where(x => x.Id > 2).Build());
            var result3 = await repository.GetExistsAsync(new QueryBuilder<Topping>().Where(x => x.AdditionalCost < -1).Build());

            Assert.True(result1);
            Assert.True(result2);
            Assert.False(result3);
        }

        [Fact]
        public async Task GetExistsAsyncFilterTest()
        {
            var options = GetOptions();
            var repository = GetReadOnlyRepository(options);
            await AddDefaultMenuItems(options);

            var result1 = await repository.GetExistsAsync<Topping>(x => x.Name.Equals("Bacon"));
            var result2 = await repository.GetExistsAsync<Topping>(x => x.Id > 2);
            var result3 = await repository.GetExistsAsync<Topping>(x => x.AdditionalCost < -1);

            Assert.True(result1);
            Assert.True(result2);
            Assert.False(result3);
        }

        [Fact]
        public async Task ComplexIncludeTest()
        {
            var options = GetOptions();
            var repository = GetReadOnlyRepository(options);
            await AddDefaultMenuItems(options);

            var people = await repository.GetAllAsync(new QueryBuilder<Person>()
                .Include(x => x.PersonPizzas.Select(p => p.Pizza.Topping))
                .Build());

            Assert.NotNull(people?.FirstOrDefault()?.PersonPizzas?.FirstOrDefault()?.Pizza?.Topping);
            Assert.Null(people?.FirstOrDefault()?.PersonPizzas?.FirstOrDefault()?.Pizza?.Crust);
        }
    }
}
