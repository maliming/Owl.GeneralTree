using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace Owl.GeneralTree;

[DependsOn(
    typeof(GeneralTreeDomainSharedModule),
    typeof(AbpDddApplicationContractsModule)
)]
public class GeneralTreeApplicationContractsModule : AbpModule
{
}