using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace Owl.GeneralTree.MongoDB
{
    public abstract class MongoGeneralTreeRepository<TDbContext, TTree, TPrimaryKey> :
        MongoDbRepository<TDbContext, TTree, TPrimaryKey>, IGeneralTreeRepository<TTree, TPrimaryKey>
        where TPrimaryKey : struct
        where TTree : class, IGeneralTree<TTree, TPrimaryKey>
        where TDbContext : IAbpMongoDbContext
    {
        protected MongoGeneralTreeRepository(IMongoDbContextProvider<TDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<TTree> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await GetMongoQueryable()
                .Where(x => x.Name == name)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<TTree> GetLastChildrenAsync(TPrimaryKey? parentId, CancellationToken cancellationToken = default)
        {
            return await GetMongoQueryable()
                .Where(x => x.ParentId.Equals(parentId))
                //.Where(EqualParentId(parentId))
                .OrderByDescending(x => x.Code)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<TTree>> GetChildrenAsync(TPrimaryKey? parentId, CancellationToken cancellationToken = default)
        {
            return await GetMongoQueryable()
                .Where(x => x.ParentId.Equals(parentId))
                //.Where(EqualParentId(parentId))
                .ToListAsync(cancellationToken);
        }

        public async Task<List<TTree>> GetAllChildrenAsync(TPrimaryKey? parentId, CancellationToken cancellationToken = default)
        {
            if (parentId.HasValue)
            {
                var tree = await FindAsync(parentId.Value, cancellationToken: cancellationToken);
                return GetMongoQueryable()
                    .Where(x => x.Code.StartsWith(tree.Code))
                    .Where(x => !x.Id.Equals(parentId.Value))
                    //.Where(NotEqualId(parentId.Value))
                    .ToList();
            }

            return await GetListAsync(cancellationToken: cancellationToken);
        }

        public async Task<bool> CheckSameNameAsync(TPrimaryKey? parentId, string name, TPrimaryKey excludeId, CancellationToken cancellationToken = default)
        {
            return await GetMongoQueryable()
                .Where(x => x.ParentId.Equals(parentId))
                //.Where(EqualParentId(parentId))
                .Where(x => !x.Id.Equals(excludeId))
                //.Where(NotEqualId(excludeId))
                .AnyAsync(x => x.Name == name, cancellationToken);
        }

        #region EqualExpression

        protected virtual FilterDefinition<TTree> CreateEqualIdFilter(TPrimaryKey id, bool applyFilters = false)
        {
            var filters = new List<FilterDefinition<TTree>>
            {
                Builders<TTree>.Filter.Eq(e => e.Id, id)
            };

            if (applyFilters)
            {
                AddGlobalFilters(filters);
            }

            return Builders<TTree>.Filter.And(filters);
        }

        protected virtual FilterDefinition<TTree> CreateNotEqualIdFilter(TPrimaryKey id, bool applyFilters = false)
        {
            var filters = new List<FilterDefinition<TTree>>
            {
                Builders<TTree>.Filter.Ne(e => e.Id, id)
            };

            if (applyFilters)
            {
                AddGlobalFilters(filters);
            }

            return Builders<TTree>.Filter.And(filters);
        }

        protected virtual FilterDefinition<TTree> CreateEqualParentIdFilter(TPrimaryKey id, bool applyFilters = false)
        {
            var filters = new List<FilterDefinition<TTree>>
            {
                Builders<TTree>.Filter.Eq(e => e.ParentId, id)
            };

            if (applyFilters)
            {
                AddGlobalFilters(filters);
            }

            return Builders<TTree>.Filter.And(filters);
        }

        protected virtual FilterDefinition<TTree> CreateNotEqualParentIdFilter(TPrimaryKey id, bool applyFilters = false)
        {
            var filters = new List<FilterDefinition<TTree>>
            {
                Builders<TTree>.Filter.Ne(e => e.ParentId, id)
            };

            if (applyFilters)
            {
                AddGlobalFilters(filters);
            }

            return Builders<TTree>.Filter.And(filters);
        }
        #endregion
    }
}
