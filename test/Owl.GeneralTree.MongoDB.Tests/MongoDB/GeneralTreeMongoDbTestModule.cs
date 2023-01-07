using System;
using Mongo2Go;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Modularity;

namespace Owl.GeneralTree.MongoDB;

[DependsOn(
    typeof(GeneralTreeTestBaseModule),
    typeof(GeneralTreeMongoDbModule)
)]
public class GeneralTreeMongoDbTestModule : AbpModule
{
    private readonly MongoDbRunner _mongoDbRunner = MongoDbRunner.Start();

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var connectionString = _mongoDbRunner.ConnectionString.EnsureEndsWith('/') +
                               "Db_" +
                               Guid.NewGuid().ToString("N");

        Configure<AbpDbConnectionOptions>(options =>
        {
            options.ConnectionStrings.Default = connectionString;
        });
    }

    public override void OnApplicationShutdown(ApplicationShutdownContext context)
    {
        _mongoDbRunner.Dispose();
    }
}