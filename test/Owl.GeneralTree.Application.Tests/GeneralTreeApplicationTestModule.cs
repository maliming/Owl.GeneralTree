using Volo.Abp.Modularity;

namespace Owl.GeneralTree;

[DependsOn(
    typeof(GeneralTreeApplicationModule),
    typeof(GeneralTreeDomainTestModule)
)]
public class GeneralTreeApplicationTestModule : AbpModule
{

}