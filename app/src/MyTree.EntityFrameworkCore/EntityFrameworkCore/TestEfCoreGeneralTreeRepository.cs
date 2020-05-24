using System;
using Owl.GeneralTree;
using Owl.GeneralTree.EntityFrameworkCore;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;

namespace MyTree.EntityFrameworkCore
{
    [ExposeServices(typeof(IGeneralTreeRepository<Region, Guid>))]
    public class MyTreeEfCoreGeneralTreeRepository : EfCoreGeneralTreeRepository<MyTreeDbContext, Region, Guid>
    {
        public MyTreeEfCoreGeneralTreeRepository(IDbContextProvider<MyTreeDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }
}
