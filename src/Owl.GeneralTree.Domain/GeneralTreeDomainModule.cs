using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Owl.GeneralTree;

[DependsOn(typeof(GeneralTreeDomainSharedModule))]
public class GeneralTreeDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddTransient<IGeneralTreeCodeGenerator, GeneralTreeCodeGenerator>();
        context.Services.AddTransient(typeof(IGeneralTreeManager<,>), typeof(GeneralTreeManager<,>));
    }
}