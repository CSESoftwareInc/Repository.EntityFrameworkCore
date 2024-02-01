using CSESoftware.Repository.EntityFrameworkCore.TestProject.Setup;

namespace CSESoftware.Repository.EntityFrameworkCore.TestProject
{
    public class DeleteTests : BaseTest
    {
        [Fact]
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
            Assert.Equal(false, await readRepository.GetExistsAsync<Topping>(x => x.Id == 1));
            Assert.Equal(0, await readRepository.GetCountAsync<Topping>());
        }

        [Fact]
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
            Assert.Equal(false, await readRepository.GetExistsAsync<Topping>(x => x.Id == 1));
            Assert.Equal(0, await readRepository.GetCountAsync<Topping>());
        }

        [Fact]
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
            createRepository.Create(new List<Topping> { topping1, topping2, topping3 });
            await createRepository.SaveAsync();

            var deleteRepository = GetRepository(options);
            var toppingsToDelete = await deleteRepository.GetAllAsync<Topping>(x => x.AdditionalCost > 2);
            deleteRepository.Delete(toppingsToDelete);
            await deleteRepository.SaveAsync();

            var readRepository = GetRepository(options);
            Assert.Equal(topping2.Id, (await readRepository.GetFirstAsync<Topping>()).Id);
            Assert.Equal(1, await readRepository.GetCountAsync<Topping>());
        }

        [Fact]
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
            createRepository.Create(new List<Topping> { topping1, topping2, topping3 });
            await createRepository.SaveAsync();

            var deleteRepository = GetRepository(options);
            deleteRepository.Delete<Topping>(x => x.AdditionalCost > 2);
            await deleteRepository.SaveAsync();

            var readRepository = GetRepository(options);
            Assert.Equal(topping2.Id, (await readRepository.GetFirstAsync<Topping>()).Id);
            Assert.Equal(1, await readRepository.GetCountAsync<Topping>());
        }
    }
}
