using System.Threading.Tasks;

namespace MyTree.Data
{
    public interface IMyTreeDbSchemaMigrator
    {
        Task MigrateAsync();
    }
}
