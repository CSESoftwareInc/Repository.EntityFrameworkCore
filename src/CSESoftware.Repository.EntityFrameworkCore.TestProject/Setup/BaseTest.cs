using Microsoft.EntityFrameworkCore;

namespace CSESoftware.Repository.EntityFrameworkCore.TestProject.Setup
{
    public class BaseTest
    {
        public static DbContextOptions<DbContext> GetOptions()
        {
            return new DbContextOptionsBuilder<DbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        }

        public static Repository<DbContext> GetRepository(DbContextOptions options)
        {
            return new Repository<DbContext>(new DbContext(options));
        }
    }
}
