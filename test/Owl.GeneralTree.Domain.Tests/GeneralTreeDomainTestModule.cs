using Owl.GeneralTree.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Owl.GeneralTree;

[DependsOn(
    typeof(GeneralTreeEntityFrameworkCoreTestModule)
)]
public class GeneralTreeDomainTestModule : AbpModule
{

}