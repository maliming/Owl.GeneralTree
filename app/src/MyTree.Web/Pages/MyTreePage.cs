using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using MyTree.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace MyTree.Web.Pages
{
    /* Inherit your UI Pages from this class. To do that, add this line to your Pages (.cshtml files under the Page folder):
     * @inherits MyTree.Web.Pages.MyTreePage
     */
    public abstract class MyTreePage : AbpPage
    {
        [RazorInject]
        public IHtmlLocalizer<MyTreeResource> L { get; set; }
    }
}
