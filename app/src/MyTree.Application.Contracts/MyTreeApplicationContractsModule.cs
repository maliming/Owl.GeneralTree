using Owl.GeneralTree;
using Volo.Abp.Modularity;

namespace MyTree
{
    [DependsOn(
        typeof(MyTreeDomainSharedModule),
        typeof(GeneralTreeApplicationContractsModule)
    )]
    public class MyTreeApplicationContractsModule : AbpModule
    {

    }
}
