using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MyTree.EntityFrameworkCore
{
    /* This class is needed for EF Core console commands
     * (like Add-Migration and Update-Database commands) */
    public class MyTreeMigrationsDbContextFactory : IDesignTimeDbContextFactory<MyTreeMigrationsDbContext>
    {
        public MyTreeMigrationsDbContext CreateDbContext(string[] args)
        {
            MyTreeEfCoreEntityExtensionMappings.Configure();

            var configuration = BuildConfiguration();

            var builder = new DbContextOptionsBuilder<MyTreeMigrationsDbContext>()
                .UseSqlServer(configuration.GetConnectionString("Default"));

            return new MyTreeMigrationsDbContext(builder.Options);
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

            return builder.Build();
        }
    }
}
