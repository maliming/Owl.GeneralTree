using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Owl.GeneralTree
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(AbpTestBaseModule)
    )]
    public class GeneralTreeTestBaseModule : AbpModule
    {
    }
}