using Volo.Abp.Modularity;
using Volo.Abp.Localization;
using Owl.GeneralTree.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace Owl.GeneralTree
{
    [DependsOn(
        typeof(AbpLocalizationModule)
    )]
    public class GeneralTreeDomainSharedModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.AddEmbedded<GeneralTreeDomainSharedModule>("Owl.GeneralTree");
            });

            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Add<GeneralTreeResource>("en")
                    .AddBaseTypes(typeof(AbpValidationResource))
                    .AddVirtualJson("/Localization/GeneralTree");
            });

            Configure<AbpExceptionLocalizationOptions>(options =>
            {
                options.MapCodeNamespace("GeneralTree", typeof(GeneralTreeResource));
            });
        }
    }
}
