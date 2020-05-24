using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

namespace MyTree.EntityFrameworkCore
{
    [ConnectionStringName("Default")]
    public class MyTreeDbContext : AbpDbContext<MyTreeDbContext>
    {
        public DbSet<Region> Regions { get; set; }

        public MyTreeDbContext(DbContextOptions<MyTreeDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Region>(b =>
            {
                b.ToTable(MyTreeConsts.DbTablePrefix + "Regions", MyTreeConsts.DbSchema);
            });
        }
    }
}
