using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSESoftware.Repository.Builder;
using CSESoftware.Repository.EntityFrameworkCore.TestProject.Setup;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DbContext = CSESoftware.Repository.EntityFrameworkCore.TestProject.Setup.DbContext;

namespace CSESoftware.Repository.EntityFrameworkCore.TestProject
{
    [TestClass]
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
                AdditionalCharge = 1,
            },
            new Crust
            {
                Id = 2,
                Name = "Thin Crust",
                AdditionalCharge = 0,
            },
            new Crust
            {
                Id = 3,
                Name = "Regular Crust",
                AdditionalCharge = 0,
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

            foreach (var crust in Crusts)
                repo.Create(crust);

            foreach (var topping in Toppings)
                repo.Create(topping);

            foreach (var pizza in Pizzas)
                repo.Create(pizza);

            await repo.SaveAsync();
        }

        #endregion

        [TestMethod]
        public async Task GetAllAsyncTest()
        {
            var options = GetOptions();
            var repository = GetReadOnlyRepository(options);
            await AddDefaultMenuItems(options);


            var allToppings = await repository.GetAllAsync<Topping>();
            Assert.AreEqual(4, allToppings.Count());


            var extraCostToppings = await repository.GetAllAsync<Topping>(x => x.AdditionalCost > 0);
            Assert.AreEqual(1, extraCostToppings.Count());


            // Where
            var freeToppings = new QueryBuilder<Topping>().Where(x => x.AdditionalCost.Equals(0)).Build();
            var inactiveToppings = await repository.GetAllAsync(freeToppings);
            Assert.AreEqual(3, inactiveToppings.Count());


            // OrderBy
            var pizzasOrderedByCostQuery = new QueryBuilder<Pizza>().OrderBy( x => x.OrderBy(y => y.Cost)).Build();
            var pizzasOrderedByCost = (await repository.GetAllAsync(pizzasOrderedByCostQuery)).ToList();
            Assert.AreEqual(8, pizzasOrderedByCost[0].Cost);
            Assert.AreEqual(10, pizzasOrderedByCost[1].Cost);
            Assert.AreEqual(12, pizzasOrderedByCost[2].Cost);


            // Include
            var repository2 = GetReadOnlyRepository(options);
            var includeQuery = new QueryBuilder<Pizza>().Include(x => x.Topping).Build();
            var pizzasWithTopping = (await repository2.GetAllAsync(includeQuery)).ToList();

            Assert.IsNotNull(pizzasWithTopping.FirstOrDefault()?.Topping);
            Assert.IsNull(pizzasWithTopping.FirstOrDefault()?.Crust);


            // Skip
            var skipToppingsQuery = new QueryBuilder<Topping>().Skip(2).Build();
            var toppingsSkip2 = (await repository.GetAllAsync(skipToppingsQuery)).ToList();
            Assert.AreEqual(3, toppingsSkip2.FirstOrDefault()?.Id);
            Assert.AreEqual(2, toppingsSkip2.Count);


            // Take
            var takeToppingsQuery = new QueryBuilder<Topping>().Take(2).Build();
            var toppingsTake2 = (await repository.GetAllAsync(takeToppingsQuery)).ToList();
            Assert.AreEqual(1, toppingsTake2.FirstOrDefault()?.Id);
            Assert.AreEqual(2, toppingsTake2.Count);
        }


        [TestMethod]
        public async Task GetAllWithSelectAsyncTest()
        {
            var options = GetOptions();
            var repository = GetReadOnlyRepository(options);
            await AddDefaultMenuItems(options);


            var toppings = await repository.GetAllWithSelectAsync(
                new QueryBuilder<Topping>().Select(x => x.Name).Build());

            var firstTopping = (string) toppings.FirstOrDefault();
            Assert.AreEqual("Bacon", firstTopping);
        }

        [TestMethod]
        public async Task GetFirstAsyncTest()
        {
            var options = GetOptions();
            var repository = GetReadOnlyRepository(options);
            await AddDefaultMenuItems(options);

            var result = await repository.GetFirstAsync<Pizza>();

            Assert.AreEqual("Pan Sausage", result.Name);
        }

        [TestMethod]
        public async Task GetCountAsyncTest()
        {
            var options = GetOptions();
            var repository = GetReadOnlyRepository(options);
            await AddDefaultMenuItems(options);

            var result1 = await repository.GetCountAsync<Topping>();
            var result2 = await repository.GetCountAsync<Topping>(x => x.Id == 1);
            var result3 = await repository.GetCountAsync<Topping>(x => x.AdditionalCost > 0);

            Assert.AreEqual(4, result1);
            Assert.AreEqual(1, result2);
            Assert.AreEqual(1, result3);
        }

        [TestMethod]
        public async Task GetExistsAsyncTest()
        {
            var options = GetOptions();
            var repository = GetReadOnlyRepository(options);
            await AddDefaultMenuItems(options);

            var result1 = await repository.GetExistsAsync<Topping>(x => x.Name.Equals("Bacon"));
            var result2 = await repository.GetExistsAsync<Topping>(x => x.Id > 2);
            var result3 = await repository.GetExistsAsync<Topping>(x => x.AdditionalCost < -1);

            Assert.IsTrue(result1);
            Assert.IsTrue(result2);
            Assert.IsFalse(result3);
        }
    }
}