using MyTree.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace MyTree.DbMigrator
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(MyTreeEntityFrameworkCoreDbMigrationsModule),
        typeof(MyTreeApplicationContractsModule)
        )]
    public class MyTreeDbMigratorModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);
        }
    }
}
