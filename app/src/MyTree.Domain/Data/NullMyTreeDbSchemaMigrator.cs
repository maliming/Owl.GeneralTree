using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace MyTree.Data
{
    /* This is used if database provider does't define
     * IMyTreeDbSchemaMigrator implementation.
     */
    public class NullMyTreeDbSchemaMigrator : IMyTreeDbSchemaMigrator, ITransientDependency
    {
        public Task MigrateAsync()
        {
            return Task.CompletedTask;
        }
    }
}