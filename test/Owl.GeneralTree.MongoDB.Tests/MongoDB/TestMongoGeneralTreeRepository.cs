using System;
using Owl.GeneralTree.App;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MongoDB;

namespace Owl.GeneralTree.MongoDB;

[ExposeServices(typeof(IGeneralTreeRepository<Region, Guid>))]
public class TestMongoGeneralTreeRepository : MongoGeneralTreeRepository<TestDbContext, Region, Guid>
{
    public TestMongoGeneralTreeRepository(IMongoDbContextProvider<TestDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
}