using Volo.Abp.Data;
using Volo.Abp.Modularity;

namespace Owl.GeneralTree.MongoDB;

[DependsOn(
    typeof(GeneralTreeTestBaseModule),
    typeof(GeneralTreeMongoDbModule)
)]
public class GeneralTreeMongoDbTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpDbConnectionOptions>(options =>
        {
            options.ConnectionStrings.Default = MongoDbFixture.GetRandomConnectionString();
        });
    }
}
