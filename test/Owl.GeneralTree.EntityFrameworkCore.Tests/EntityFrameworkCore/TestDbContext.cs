using Microsoft.EntityFrameworkCore;
using Owl.GeneralTree.App;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Owl.GeneralTree.EntityFrameworkCore;

public class TestDbContext : AbpDbContext<TestDbContext>
{
    public DbSet<Region> Regions { get; set; }

    public TestDbContext(DbContextOptions<TestDbContext> options) 
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Region>(b =>
        {
            b.TryConfigureExtraProperties();
            b.TryConfigureConcurrencyStamp();
        });
    }
}