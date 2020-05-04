using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyTree.Data;
using Volo.Abp.DependencyInjection;

namespace MyTree.EntityFrameworkCore
{
    public class EntityFrameworkCoreMyTreeDbSchemaMigrator
        : IMyTreeDbSchemaMigrator, ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;

        public EntityFrameworkCoreMyTreeDbSchemaMigrator(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task MigrateAsync()
        {
            /* We intentionally resolving the MyTreeMigrationsDbContext
             * from IServiceProvider (instead of directly injecting it)
             * to properly get the connection string of the current tenant in the
             * current scope.
             */

            await _serviceProvider
                .GetRequiredService<MyTreeMigrationsDbContext>()
                .Database
                .MigrateAsync();
        }
    }
}