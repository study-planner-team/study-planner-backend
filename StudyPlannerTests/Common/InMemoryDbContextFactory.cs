using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.Data;

namespace StudyPlannerTests.Common
{
    public class InMemoryDbContextFactory
    {
        public static AppDbContext Create(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new AppDbContext(options);
        }
    }
}
