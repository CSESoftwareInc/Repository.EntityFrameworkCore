using CSESoftware.Repository.EntityFrameworkCore.TestProject.Setup;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSESoftware.Repository.EntityFrameworkCore.TestProject
{
    [TestClass]
    public class DeleteTests : BaseTest
    {
        [TestMethod]
        public async Task DeleteByEntityTest()
        {
            var options = GetOptions();
            var repository = GetRepository(options);

            await repository.CreateAsync(new Topping
            {
                Name = "Canadian Bacon",
                AdditionalCost = 2.5
            });

            var deleteRepository = GetRepository(options);
            await deleteRepository.DeleteAsync(await deleteRepository.GetFirstAsync<Topping>());

            var readRepository = GetRepository(options);
            Assert.AreEqual(false, await readRepository.GetExistsAsync<Topping>(x => x.Id == 1));
            Assert.AreEqual(0, await readRepository.GetCountAsync<Topping>());
        }

        [TestMethod]
        public async Task DeleteByIdTest() //todo update
        {
            var options = GetOptions();
            var createRepository = GetRepository(options);

            await createRepository.CreateAsync(new Topping
            {
                Name = "Canadian Bacon",
                AdditionalCost = 2.5
            });

            var deleteRepository = GetRepository(options);
            await deleteRepository.DeleteAsync<Topping>(x => x.Id == 1);

            var readRepository = GetRepository(options);
            Assert.AreEqual(false, await readRepository.GetExistsAsync<Topping>(x => x.Id == 1));
            Assert.AreEqual(0, await readRepository.GetCountAsync<Topping>());
        }

        [TestMethod]
        public async Task DeleteByListTest()
        {
            var options = GetOptions();

            var topping1 = new Topping
            {
                Name = "Canadian Bacon",
                AdditionalCost = 2.5
            };
            var topping2 = new Topping
            {
                Name = "Bacon",
                AdditionalCost = 1
            };
            var topping3 = new Topping
            {
                Name = "Sausage",
                AdditionalCost = 2.25
            };

            var createRepository = GetRepository(options);
            await createRepository.CreateAsync(new List<Topping>{topping1, topping2, topping3});

            var deleteRepository = GetRepository(options);
            var toppingsToDelete = await deleteRepository.GetAllAsync<Topping>(x => x.AdditionalCost > 2);
            await deleteRepository.DeleteAsync(toppingsToDelete);

            var readRepository = GetRepository(options);
            Assert.AreEqual(topping2.Id, (await readRepository.GetFirstAsync<Topping>()).Id);
            Assert.AreEqual(1, await readRepository.GetCountAsync<Topping>());
        }

        [TestMethod]
        public async Task DeleteByExpressionTest()
        {
            var options = GetOptions();

            var topping1 = new Topping
            {
                Name = "Canadian Bacon",
                AdditionalCost = 2.5
            };
            var topping2 = new Topping
            {
                Name = "Bacon",
                AdditionalCost = 1
            };
            var topping3 = new Topping
            {
                Name = "Sausage",
                AdditionalCost = 2.25
            };

            var createRepository = GetRepository(options);
            await createRepository.CreateAsync(new List<Topping>{topping1, topping2, topping3});

            var deleteRepository = GetRepository(options);
            await deleteRepository.DeleteAsync<Topping>(x => x.AdditionalCost > 2);

            var readRepository = GetRepository(options);
            Assert.AreEqual(topping2.Id, (await readRepository.GetFirstAsync<Topping>()).Id);
            Assert.AreEqual(1, await readRepository.GetCountAsync<Topping>());
        }
    }
}
