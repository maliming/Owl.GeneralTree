using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Components;
using Volo.Abp.DependencyInjection;

namespace MyTree.Web
{
    [Dependency(ReplaceServices = true)]
    public class MyTreeBrandingProvider : DefaultBrandingProvider
    {
        public override string AppName => "MyTree";
    }
}
