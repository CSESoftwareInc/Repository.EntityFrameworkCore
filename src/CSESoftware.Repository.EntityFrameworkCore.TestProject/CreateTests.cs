using CSESoftware.Repository.EntityFrameworkCore.TestProject.Setup;

namespace CSESoftware.Repository.EntityFrameworkCore.TestProject
{
    public class CreateTests : BaseTest
    {
        [Fact]
        public async Task CreateOneTest()
        {
            var options = GetOptions();
            var createRepository = GetRepository(options);

            createRepository.Create(new Topping
            {
                Name = "Canadian Bacon",
                AdditionalCost = 2.5,
                IsActive = false
            });
            await createRepository.SaveAsync();

            var readRepository = GetRepository(options);
            var result = (await readRepository.GetAllAsync<Topping>()).ToList();

            Assert.Equal(1, result.Count);
            Assert.Equal("Canadian Bacon", result.FirstOrDefault()?.Name);
            Assert.Equal(2.5, result.FirstOrDefault()?.AdditionalCost);
            Assert.True(result.FirstOrDefault()?.IsActive ?? false);
            Assert.True(result.FirstOrDefault()?.CreatedDate > DateTime.UtcNow.AddSeconds(-3));
            Assert.True(result.FirstOrDefault()?.ModifiedDate > DateTime.UtcNow.AddSeconds(-3));
            Assert.True(result.First().IsActive);
        }

        [Fact]
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

            Assert.Equal(1, result.Count);
            Assert.Equal("Pan", result.FirstOrDefault()?.Name);
            Assert.Equal(2.5, result.FirstOrDefault()?.AdditionalCost);
        }

        [Fact]
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

            var toppings = new List<Topping> { topping1, topping2, topping3, topping4 };

            var createRepository = GetRepository(options);
            createRepository.Create(toppings);
            await createRepository.SaveAsync();

            var readRepository = GetRepository(options);
            var result = (await readRepository.GetAllAsync<Topping>()).ToList();

            Assert.Equal(4, result.Count);
            Assert.Equal(2, result.Count(x => Math.Abs(x.AdditionalCost - 0.75) < 0.001));
            Assert.False(result.Any(x => !x.IsActive));
            Assert.False(result.Any(x => x.CreatedDate < DateTime.UtcNow.AddSeconds(-3)));
        }

        [Fact]
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

            var crusts = new List<Crust> { crust1, crust2, crust3, crust4 };

            var createRepository = GetRepository(options);
            createRepository.Create(crusts);
            await createRepository.SaveAsync();

            var readRepository = GetRepository(options);
            var result = (await readRepository.GetAllAsync<Crust>()).ToList();

            Assert.Equal(4, result.Count);
            Assert.Equal(2, result.Count(x => Math.Abs(x.AdditionalCost - 0.75) < 0.001));
        }
    }
}
