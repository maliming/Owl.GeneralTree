using Volo.Abp.Settings;

namespace MyTree.Settings
{
    public class MyTreeSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            //Define your own settings here. Example:
            //context.Add(new SettingDefinition(MyTreeSettings.MySetting1));
        }
    }
}
