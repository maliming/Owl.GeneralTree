using Volo.Abp.Application;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace Owl.GeneralTree
{
    [DependsOn(
        typeof(GeneralTreeDomainModule),
        typeof(GeneralTreeApplicationContractsModule),
        typeof(AbpDddApplicationModule),
        typeof(AbpAutoMapperModule)
    )]
    public class GeneralTreeApplicationModule : AbpModule
    {

    }
}
