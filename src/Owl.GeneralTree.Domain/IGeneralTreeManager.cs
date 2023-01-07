using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Uow;

namespace Owl.GeneralTree;

public interface IGeneralTreeManager<TTree, TPrimaryKey> : IUnitOfWorkEnabled
    where TPrimaryKey : struct
    where TTree : class, IGeneralTree<TTree, TPrimaryKey>
{
    Task CreateAsync(TTree tree, CancellationToken cancellationToken = default);

    Task BulkCreateAsync(TTree tree, Action<TTree> childrenAction = null, CancellationToken cancellationToken = default);

    Task BulkCreateChildrenAsync(TTree parent, ICollection<TTree> children, Action<TTree> childrenAction = null, CancellationToken cancellationToken = default);

    Task FillUpAsync(TTree tree, Action<TTree> childrenAction = null, CancellationToken cancellationToken = default);

    Task UpdateNameAsync(TTree tree, Action<TTree> childrenAction = null, CancellationToken cancellationToken = default);

    Task MoveToBeforeAsync(TPrimaryKey id, TPrimaryKey afterId, Action<TTree> childrenAction = null, CancellationToken cancellationToken = default);

    Task MoveToAsync(TPrimaryKey id, TPrimaryKey? parentId, Action<TTree> childrenAction = null, CancellationToken cancellationToken = default);

    Task DeleteAsync(TPrimaryKey id, CancellationToken cancellationToken = default);

    Task RegenerateAsync(TPrimaryKey? parentId = null, CancellationToken cancellationToken = default);
}