using System;
using Owl.GeneralTree.App;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;

namespace Owl.GeneralTree.EntityFrameworkCore
{
    [ExposeServices(typeof(IGeneralTreeRepository<Region, Guid>))]
    public class TestEfCoreGeneralTreeRepository : EfCoreGeneralTreeRepository<TestDbContext, Region, Guid>
    {
        public TestEfCoreGeneralTreeRepository(IDbContextProvider<TestDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }
}