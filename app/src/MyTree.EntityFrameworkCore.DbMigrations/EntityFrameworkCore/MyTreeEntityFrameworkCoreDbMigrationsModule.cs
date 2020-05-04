using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace MyTree.EntityFrameworkCore
{
    [DependsOn(
        typeof(MyTreeEntityFrameworkCoreModule)
        )]
    public class MyTreeEntityFrameworkCoreDbMigrationsModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<MyTreeMigrationsDbContext>();
        }
    }
}
