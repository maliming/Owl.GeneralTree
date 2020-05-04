using Microsoft.EntityFrameworkCore;
using Volo.Abp;

namespace MyTree.EntityFrameworkCore
{
    public static class MyTreeDbContextModelCreatingExtensions
    {
        public static void ConfigureMyTree(this ModelBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            /* Configure your own tables/entities inside here */

            //builder.Entity<YourEntity>(b =>
            //{
            //    b.ToTable(MyTreeConsts.DbTablePrefix + "YourEntities", MyTreeConsts.DbSchema);

            //    //...
            //});
        }
    }
}