using MongoDB.Driver;
using Owl.GeneralTree.App;
using Volo.Abp.MongoDB;

namespace Owl.GeneralTree.MongoDB;

public class TestDbContext : AbpMongoDbContext
{
    public IMongoCollection<Region> Regions => Collection<Region>();
}