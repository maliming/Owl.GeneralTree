using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Owl.GeneralTree
{
    public interface IGeneralTreeRepository<TTree, TPrimaryKey> : IBasicRepository<TTree, TPrimaryKey>
        where TPrimaryKey : struct
        where TTree : class, IGeneralTree<TTree, TPrimaryKey>
    {
        Task<TTree> GetByNameAsync(string name, CancellationToken cancellationToken = default);

        Task DeleteAsync(Expression<Func<TTree, bool>> predicate, bool autoSave = false, CancellationToken cancellationToken = default);

        Task<TTree> GetLastChildrenAsync(TPrimaryKey? parentId, CancellationToken cancellationToken = default);

        Task<List<TTree>> GetChildrenAsync(TPrimaryKey? parentId, CancellationToken cancellationToken = default);

        Task<List<TTree>> GetAllChildrenAsync(TPrimaryKey? parentId, CancellationToken cancellationToken = default);

        Task<bool> CheckSameNameAsync(TPrimaryKey? parentId, string name, TPrimaryKey excludeId, CancellationToken cancellationToken = default);
    }
}
