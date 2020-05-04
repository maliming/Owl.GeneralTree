using MyTree.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace MyTree.Permissions
{
    public class MyTreePermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var myGroup = context.AddGroup(MyTreePermissions.GroupName);

            //Define your own permissions here. Example:
            //myGroup.AddPermission(MyTreePermissions.MyPermission1, L("Permission:MyPermission1"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<MyTreeResource>(name);
        }
    }
}
