using MyTree.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace MyTree
{
    [DependsOn(
        typeof(MyTreeEntityFrameworkCoreTestModule)
        )]
    public class MyTreeDomainTestModule : AbpModule
    {

    }
}