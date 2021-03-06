using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace MyTree.Web
{
    [Dependency(ReplaceServices = true)]
    public class MyTreeBrandingProvider : DefaultBrandingProvider
    {
        public override string AppName => "MyTree";
    }
}
