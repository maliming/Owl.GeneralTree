using Owl.GeneralTree;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace MyTree
{
    [DependsOn(
        typeof(AbpAutoMapperModule),
        typeof(MyTreeDomainModule),
        typeof(MyTreeApplicationContractsModule),
        typeof(GeneralTreeApplicationModule)
    )]
    public class MyTreeApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<MyTreeApplicationModule>();
            });
        }
    }
}
