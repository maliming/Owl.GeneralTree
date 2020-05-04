using Volo.Abp.Modularity;

namespace MyTree
{
    [DependsOn(
        typeof(MyTreeApplicationModule),
        typeof(MyTreeDomainTestModule)
        )]
    public class MyTreeApplicationTestModule : AbpModule
    {

    }
}