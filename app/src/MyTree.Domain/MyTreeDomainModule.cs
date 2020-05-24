using Owl.GeneralTree;
using Volo.Abp.Modularity;

namespace MyTree
{
    [DependsOn(
        typeof(MyTreeDomainSharedModule),
        typeof(GeneralTreeDomainModule)
    )]
    public class MyTreeDomainModule : AbpModule
    {

    }
}
