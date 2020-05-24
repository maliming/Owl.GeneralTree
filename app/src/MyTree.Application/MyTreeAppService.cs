using MyTree.Localization;
using Volo.Abp.Application.Services;

namespace MyTree
{
    /* Inherit your application services from this class.
     */
    public abstract class MyTreeAppService : ApplicationService
    {
        protected MyTreeAppService()
        {
            LocalizationResource = typeof(MyTreeResource);
        }
    }
}
