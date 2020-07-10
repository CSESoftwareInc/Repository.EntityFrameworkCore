using System.Collections.Generic;
using System.Threading.Tasks;
using CSESoftware.Repository.EntityFrameworkCore.TestProject.Setup;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

            repository.Create(new Topping
            {
                Name = "Canadian Bacon",
                AdditionalCost = 2.5
            });
            await repository.SaveAsync();


            var deleteRepository = GetRepository(options);
            deleteRepository.Delete(await deleteRepository.GetFirstAsync<Topping>());
            await deleteRepository.SaveAsync();


            var readRepository = GetRepository(options);
            Assert.AreEqual(false, await readRepository.GetExistsAsync<Topping>(x => x.Id == 1));
            Assert.AreEqual(0, await readRepository.GetCountAsync<Topping>());
        }

        [TestMethod]
        public async Task DeleteByIdTest()
        {
            var options = GetOptions();
            var createRepository = GetRepository(options);

            createRepository.Create(new Topping
            {
                Name = "Canadian Bacon",
                AdditionalCost = 2.5
            });
            await createRepository.SaveAsync();


            var deleteRepository = GetRepository(options);
            deleteRepository.Delete<Topping, int>(1);
            await deleteRepository.SaveAsync();


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
            createRepository.Create(new List<Topping>{topping1, topping2, topping3});
            await createRepository.SaveAsync();


            var deleteRepository = GetRepository(options);
            var toppingsToDelete = await deleteRepository.GetAllAsync<Topping>(x => x.AdditionalCost > 2);
            deleteRepository.Delete(toppingsToDelete);
            await deleteRepository.SaveAsync();


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
            createRepository.Create(new List<Topping>{topping1, topping2, topping3});
            await createRepository.SaveAsync();


            var deleteRepository = GetRepository(options);
            deleteRepository.Delete<Topping>(x => x.AdditionalCost > 2);
            await deleteRepository.SaveAsync();


            var readRepository = GetRepository(options);
            Assert.AreEqual(topping2.Id, (await readRepository.GetFirstAsync<Topping>()).Id);
            Assert.AreEqual(1, await readRepository.GetCountAsync<Topping>());
        }
    }
}
