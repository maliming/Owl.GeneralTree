using MyTree.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace MyTree.Controllers
{
    /* Inherit your controllers from this class.
     */
    public abstract class MyTreeController : AbpController
    {
        protected MyTreeController()
        {
            LocalizationResource = typeof(MyTreeResource);
        }
    }
}