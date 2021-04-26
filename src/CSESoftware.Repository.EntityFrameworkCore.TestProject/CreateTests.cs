using CSESoftware.Repository.EntityFrameworkCore.TestProject.Setup;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSESoftware.Repository.EntityFrameworkCore.TestProject
{
    [TestClass]
    public class CreateTests : BaseTest
    {
        [TestMethod]
        public async Task CreateOneTest()
        {
            var options = GetOptions();
            var createRepository = GetRepository(options);

            createRepository.Create(new Topping
            {
                Name = "Canadian Bacon",
                AdditionalCost = 2.5,
                IsActive = true
            });
            await createRepository.SaveAsync();

            var readRepository = GetRepository(options);
            var result = (await readRepository.GetAllAsync<Topping>()).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Canadian Bacon", result.FirstOrDefault()?.Name);
            Assert.AreEqual(2.5, result.FirstOrDefault()?.AdditionalCost);
            Assert.IsTrue(result.FirstOrDefault()?.IsActive ?? false);
        }

        [TestMethod]
        public async Task CreateOneNoActiveOrDateChangeTest()
        {
            var options = GetOptions();
            var createRepository = GetRepository(options);

            createRepository.Create(new Crust
            {
                Name = "Pan",
                AdditionalCost = 2.5,
            });
            await createRepository.SaveAsync();

            var readRepository = GetRepository(options);
            var result = (await readRepository.GetAllAsync<Crust>()).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Pan", result.FirstOrDefault()?.Name);
            Assert.AreEqual(2.5, result.FirstOrDefault()?.AdditionalCost);
        }

        [TestMethod]
        public async Task CreateManyTest()
        {
            var options = GetOptions();

            var topping1 = new Topping
            {
                Name = "Sausage",
                AdditionalCost = 0.5,
                IsActive = false
            };
            var topping2 = new Topping
            {
                Name = "Bacon",
                AdditionalCost = 0.75,
                IsActive = false
            };
            var topping3 = new Topping
            {
                Name = "Canadian Bacon",
                AdditionalCost = 0.75
            };
            var topping4 = new Topping
            {
                Name = "Olives",
                AdditionalCost = 1.25
            };

            var toppings = new List<Topping> {topping1, topping2, topping3, topping4};

            var createRepository = GetRepository(options);
            createRepository.Create(toppings);
            await createRepository.SaveAsync();

            var readRepository = GetRepository(options);
            var result = (await readRepository.GetAllAsync<Topping>()).ToList();

            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(2, result.Count(x => Math.Abs(x.AdditionalCost - 0.75) < 0.001));
        }

        [TestMethod]
        public async Task CreateManyNoActiveOrDateChangeTest()
        {
            var options = GetOptions();

            var crust1 = new Crust
            {
                Name = "Thin",
                AdditionalCost = 0.5
            };
            var crust2 = new Crust
            {
                Name = "Crispy",
                AdditionalCost = 0.75
            };
            var crust3 = new Crust
            {
                Name = "Thin & Crispy",
                AdditionalCost = 0.75
            };
            var crust4 = new Crust
            {
                Name = "Pan",
                AdditionalCost = 1.25
            };

            var crusts = new List<Crust> {crust1, crust2, crust3, crust4};

            var createRepository = GetRepository(options);
            createRepository.Create(crusts);
            await createRepository.SaveAsync();

            var readRepository = GetRepository(options);
            var result = (await readRepository.GetAllAsync<Crust>()).ToList();

            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(2, result.Count(x => Math.Abs(x.AdditionalCost - 0.75) < 0.001));
        }
    }
}
