using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Owl.GeneralTree.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;


namespace MyTree.EntityFrameworkCore
{
    [DependsOn(
        typeof(MyTreeDomainModule),
        typeof(GeneralTreeEntityFrameworkCoreModule)
    )]
    public class MyTreeEntityFrameworkCoreModule : AbpModule
    {
        private readonly SqliteConnection _sqliteConnection = new SqliteConnection("Data Source=:memory:");

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<MyTreeDbContext>(options =>
            {
                options.AddDefaultRepositories(includeAllEntities: true);
            });

            _sqliteConnection.Open();
            context.Services.Configure<AbpDbContextOptions>(options =>
            {
                options.Configure(x => x.DbContextOptions.UseSqlite(_sqliteConnection));
            });
        }
    }
}
