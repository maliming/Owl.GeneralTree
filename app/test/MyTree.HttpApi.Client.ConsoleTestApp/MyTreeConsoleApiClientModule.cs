using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace MyTree.HttpApi.Client.ConsoleTestApp
{
    [DependsOn(
        typeof(MyTreeHttpApiClientModule),
        typeof(AbpHttpClientIdentityModelModule)
        )]
    public class MyTreeConsoleApiClientModule : AbpModule
    {
        
    }
}
