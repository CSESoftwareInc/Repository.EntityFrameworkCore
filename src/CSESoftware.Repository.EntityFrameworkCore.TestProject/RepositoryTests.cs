using System;
using System.Linq;
using System.Threading.Tasks;
using CSESoftware.Repository.EntityFrameworkCore.TestProject.Setup;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DbContext = CSESoftware.Repository.EntityFrameworkCore.TestProject.Setup.DbContext;

namespace CSESoftware.Repository.EntityFrameworkCore.TestProject
{
    [TestClass]
    public class RepositoryTests
    {
        #region Repository Setup

        private static DbContextOptions<DbContext> GetOptions()
        {
            return new DbContextOptionsBuilder<DbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        }

        private static Repository<DbContext> GetRepository(DbContextOptions options)
        {
            return new Repository<DbContext>(new DbContext(options));
        }

        #endregion

        [TestMethod]
        public async Task CreateTest()
        {
            var options = GetOptions();
            var repository = GetRepository(options);

            repository.Create(new Topping
            {
                Name = "Canadian Bacon",
                AdditionalCost = 2.5
            });
            await repository.SaveAsync();

            var repository2 = GetRepository(options);
            var result = (await repository2.GetAllAsync<Topping>()).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Canadian Bacon", result.FirstOrDefault()?.Name);
            Assert.AreEqual(2.5, result.FirstOrDefault()?.AdditionalCost);
            Assert.IsTrue(result.FirstOrDefault()?.CreatedDate > DateTime.UtcNow.AddSeconds(-3));
            Assert.IsTrue(result.FirstOrDefault()?.ModifiedDate > DateTime.UtcNow.AddSeconds(-3));
            Assert.IsTrue(result.First().IsActive);
        }

        [TestMethod]
        public async Task UpdateTest()
        {
            var options = GetOptions();
            var repository = GetRepository(options);

            repository.Create(new Topping
            {
                Name = "Canadian Bacon",
                AdditionalCost = 2.5
            });
            await repository.SaveAsync();


            var repository2 = GetRepository(options);
            var topping = await repository2.GetFirstAsync<Topping>();
            topping.Name = "Super Canadian Bacon";
            repository2.Update(topping);
            await repository2.SaveAsync();


            var repository3 = GetRepository(options);
            var updatedTopping = await repository3.GetFirstAsync<Topping>();

            Assert.AreEqual("Super Canadian Bacon", updatedTopping.Name);
            Assert.AreEqual(2.5, updatedTopping.AdditionalCost);
        }

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


            var repository2 = GetRepository(options);
            repository2.Delete(await repository2.GetFirstAsync<Topping>());
            await repository2.SaveAsync();


            var repository3 = GetRepository(options);
            Assert.AreEqual(false, await repository3.GetExistsAsync<Topping>(x => x.Id == 1));
            Assert.AreEqual(0, await repository3.GetCountAsync<Topping>());
        }

        [TestMethod]
        public async Task DeleteByIdTest()
        {
            var options = GetOptions();
            var repository = GetRepository(options);

            repository.Create(new Topping
            {
                Name = "Canadian Bacon",
                AdditionalCost = 2.5
            });
            await repository.SaveAsync();


            var repository2 = GetRepository(options);
            repository2.Delete<Topping>(1);
            await repository2.SaveAsync();


            var repository3 = GetRepository(options);
            Assert.AreEqual(false, await repository3.GetExistsAsync<Topping>(x => x.Id == 1));
            Assert.AreEqual(0, await repository3.GetCountAsync<Topping>());
        }
    }
}