using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Owl.GeneralTree.EntityFrameworkCore
{
    [DependsOn(typeof(GeneralTreeDomainModule), typeof(AbpEntityFrameworkCoreModule))]
    public class GeneralTreeEntityFrameworkCoreModule : AbpModule
    {

    }
}
