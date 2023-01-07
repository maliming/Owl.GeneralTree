using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using MyTree.Localization;
using Volo.Abp.UI.Navigation;

namespace MyTree.Web.Menus;

public class MyTreeMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.ServiceProvider.GetRequiredService<IStringLocalizer<MyTreeResource>>();

        context.Menu.Items.Insert(0, new ApplicationMenuItem("MyTree.Home", l["Menu:Home"], "/"));
        return Task.CompletedTask;
    }
}