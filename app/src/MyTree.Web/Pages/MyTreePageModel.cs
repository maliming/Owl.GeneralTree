using MyTree.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace MyTree.Web.Pages;

/* Inherit your PageModel classes from this class.
 */
public abstract class MyTreePageModel : AbpPageModel
{
    protected MyTreePageModel()
    {
        LocalizationResourceType = typeof(MyTreeResource);
    }
}