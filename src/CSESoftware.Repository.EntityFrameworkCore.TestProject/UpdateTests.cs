using CSESoftware.Repository.EntityFrameworkCore.TestProject.Setup;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSESoftware.Repository.EntityFrameworkCore.TestProject
{
    [TestClass]
    public class UpdateTests : BaseTest
    {
        [TestMethod]
        public async Task UpdateTest()
        {
            var options = GetOptions();
            var createRepository = GetRepository(options);

            var initialTopping = new Topping
            {
                Name = "Canadian Bacon",
                AdditionalCost = 2.5
            };
            createRepository.Create(initialTopping);
            await createRepository.SaveAsync();

            var updateRepository = GetRepository(options);
            var topping = await updateRepository.GetFirstAsync<Topping>();
            topping.Name = "Super Canadian Bacon";
            updateRepository.Update(topping);
            await updateRepository.SaveAsync();

            var readRepository = GetRepository(options);
            var updatedTopping = await readRepository.GetFirstAsync<Topping>();

            Assert.AreEqual("Super Canadian Bacon", updatedTopping.Name);
            Assert.AreEqual(2.5, updatedTopping.AdditionalCost);
        }

        [TestMethod]
        public async Task UpdateNoActiveOrDateChangeTest()
        {
            var options = GetOptions();
            var createRepository = GetRepository(options);

            var initialCrust = new Crust
            {
                Name = "Thin",
                AdditionalCost = 2.5
            };
            createRepository.Create(initialCrust);
            await createRepository.SaveAsync();

            var updateRepository = GetRepository(options);
            var crust = await updateRepository.GetFirstAsync<Crust>();
            crust.Name = "Super Thin";
            updateRepository.Update(crust);
            await updateRepository.SaveAsync();

            var readRepository = GetRepository(options);
            var updatedCrust = await readRepository.GetFirstAsync<Crust>();

            Assert.AreEqual("Super Thin", updatedCrust.Name);
            Assert.AreEqual(2.5, updatedCrust.AdditionalCost);
        }

        [TestMethod]
        public async Task UpdateManyTest()
        {
            var options = GetOptions();

            var topping1 = new Topping
            {
                Name = "Sausage",
                AdditionalCost = 0.5
            };
            var topping2 = new Topping
            {
                Name = "Bacon",
                AdditionalCost = 0.75
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

            var toppings = new List<Topping> { topping1, topping2, topping3, topping4 };

            var createRepository = GetRepository(options);
            createRepository.Create(toppings);
            await createRepository.SaveAsync();

            var updateRepository = GetRepository(options);
            var updateToppings = await updateRepository.GetAllAsync<Topping>();
            updateToppings.ForEach(x => x.Name = "Tuna");
            updateRepository.Update(updateToppings);
            await updateRepository.SaveAsync();

            var readRepository = GetRepository(options);
            var result = (await readRepository.GetAllAsync<Topping>()).ToList();

            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(2, result.Count(x => Math.Abs(x.AdditionalCost - 0.75) < 0.001));
            Assert.IsTrue(result.All(x => x.Name.Equals("Tuna")));
        }

        [TestMethod]
        public async Task UpdateManyNoActiveOrDateChangeTest()
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

            var crusts = new List<Crust> { crust1, crust2, crust3, crust4 };

            var createRepository = GetRepository(options);
            createRepository.Create(crusts);
            await createRepository.SaveAsync();

            var updateRepository = GetRepository(options);
            var updateCrust = await updateRepository.GetAllAsync<Crust>();
            updateCrust.ForEach(x => x.AdditionalCost = 0);
            updateRepository.Update(updateCrust);
            await updateRepository.SaveAsync();

            var readRepository = GetRepository(options);
            var result = (await readRepository.GetAllAsync<Crust>()).ToList();

            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(4, result.Count(x => Math.Abs(x.AdditionalCost) < 0.001));
            Assert.AreEqual(1, result.Count(x => x.Name.Equals("Pan")));
        }
    }
}
