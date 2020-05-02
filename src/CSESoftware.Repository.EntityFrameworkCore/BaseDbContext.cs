using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;

namespace CSESoftware.Repository.EntityFrameworkCore
{
    public class BaseDbContext : DbContext
    {
        //Dump EF SQL to output window
        private static readonly LoggerFactory MyLoggerFactory =
            new LoggerFactory(new[]
                {
                    new DebugLoggerProvider()
                });

        public BaseDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(MyLoggerFactory); // Log SQL to output window
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking); // Disable tracking globally
        }
    }
}