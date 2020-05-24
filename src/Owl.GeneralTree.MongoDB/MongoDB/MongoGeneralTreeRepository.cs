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
                .OrderByDescending(x => x.Code)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<TTree>> GetChildrenAsync(TPrimaryKey? parentId, CancellationToken cancellationToken = default)
        {
            return await GetMongoQueryable()
                .Where(x => x.ParentId.Equals(parentId))
                .OrderBy(x => x.Code)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<TTree>> GetAllChildrenAsync(TPrimaryKey? parentId, TPrimaryKey? excludeId = null, CancellationToken cancellationToken = default)
        {
            if (parentId == null)
            {
                return await GetListAsync(cancellationToken: cancellationToken);
            }

            var tree = await GetAsync(parentId.Value, cancellationToken: cancellationToken);
            if (excludeId == null)
            {
                return await GetMongoQueryable().Where(x => x.Code.StartsWith(tree.Code))
                    .Where(x => !x.Id.Equals(parentId.Value))
                    .OrderBy(x => x.Code)
                    .ToListAsync(cancellationToken: cancellationToken);
            }

            var excludeTree = await GetAsync(parentId.Value, cancellationToken: cancellationToken);

            return await GetMongoQueryable().Where(x => x.Code.StartsWith(tree.Code))
                .Where(x => !x.Code.StartsWith(excludeTree.Code))
                .Where(x => !x.Id.Equals(parentId.Value))
                .OrderBy(x => x.Code)
                .ToListAsync(cancellationToken: cancellationToken);
        }

        public async Task<List<TTree>> GetNextAllAsync(TPrimaryKey id, TPrimaryKey? excludeId = null, CancellationToken cancellationToken = default)
        {
            var tree = await GetAsync(id, cancellationToken: cancellationToken);

            List<TTree> allChildren;
            if (tree.ParentId != null)
            {
                var parent =  await GetAsync(tree.ParentId.Value, cancellationToken: cancellationToken);
                allChildren =  await GetMongoQueryable().Where(x => x.Code.StartsWith(parent.Code))
                    .OrderBy(x => x.Code)
                    .ToListAsync(cancellationToken: cancellationToken);
            }
            else
            {
                allChildren =  await GetMongoQueryable()
                    .OrderBy(x => x.Code)
                    .ToListAsync(cancellationToken: cancellationToken);
            }

            var nextAll = allChildren.SkipWhile(x => x.Code != tree.Code);

            if (excludeId != null)
            {
                var excludeTree = await GetAsync(excludeId.Value, cancellationToken: cancellationToken);
                return nextAll.Where(x =>! x.Code.StartsWith(excludeTree.Code)).OrderBy(x => x.Code).ToList();
            }

            return nextAll.ToList();
        }

        public async Task<bool> CheckSameNameAsync(TPrimaryKey? parentId, string name, TPrimaryKey excludeId, CancellationToken cancellationToken = default)
        {
            return await GetMongoQueryable()
                .Where(x => x.ParentId.Equals(parentId))
                .Where(x => !x.Id.Equals(excludeId))
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
