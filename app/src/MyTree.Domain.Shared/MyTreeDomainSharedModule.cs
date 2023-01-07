using MyTree.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Volo.Abp.Validation.Localization;
using Volo.Abp.VirtualFileSystem;

namespace MyTree;

[DependsOn(
    typeof(AbpLocalizationModule),
    typeof(AbpValidationModule),
    typeof(AbpVirtualFileSystemModule)
)]
public class MyTreeDomainSharedModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<MyTreeDomainSharedModule>("MyTree");
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<MyTreeResource>("en")
                .AddBaseTypes(typeof(AbpValidationResource))
                .AddVirtualJson("/Localization/MyTree");
        });
    }
}