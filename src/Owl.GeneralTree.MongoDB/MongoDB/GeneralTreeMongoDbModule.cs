using Volo.Abp.Modularity;
using Volo.Abp.MongoDB;

namespace Owl.GeneralTree.MongoDB;

[DependsOn(typeof(GeneralTreeDomainModule), typeof(AbpMongoDbModule))]
public class GeneralTreeMongoDbModule : AbpModule
{

}