using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Owl.GeneralTree.App;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Owl.GeneralTree.EntityFrameworkCore
{
    [DependsOn(
        typeof(GeneralTreeTestBaseModule),
        typeof(GeneralTreeEntityFrameworkCoreModule)
        )]
    public class GeneralTreeEntityFrameworkCoreTestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<TestDbContext>(options =>
            {
                options.AddDefaultRepositories(includeAllEntities: true);
            });

            var sqliteConnection = CreateDatabaseAndGetConnection();

            Configure<AbpDbContextOptions>(options =>
            {
                options.Configure(abpDbContextConfigurationContext =>
                {
                    abpDbContextConfigurationContext.DbContextOptions.UseSqlite(sqliteConnection);
                    abpDbContextConfigurationContext.DbContextOptions.EnableSensitiveDataLogging();
                });
            });
        }

        private static SqliteConnection CreateDatabaseAndGetConnection()
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();

            new TestDbContext(
                new DbContextOptionsBuilder<TestDbContext>().UseSqlite(connection).Options
            ).GetService<IRelationalDatabaseCreator>().CreateTables();

            return connection;
        }
    }
}
