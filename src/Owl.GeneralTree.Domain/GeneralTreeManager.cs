using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Owl.GeneralTree.Localization;
using Volo.Abp;

namespace Owl.GeneralTree
{
    public class GeneralTreeManager<TTree, TPrimaryKey> : IGeneralTreeManager<TTree, TPrimaryKey>
        where TPrimaryKey : struct
        where TTree : class, IGeneralTree<TTree, TPrimaryKey>
    {
        private readonly IGeneralTreeCodeGenerator _generalTreeCodeGenerator;
        private readonly IGeneralTreeRepository<TTree, TPrimaryKey> _generalTreeRepository;
        private readonly GeneralTreeOptions _generalTreeOptions;
        private readonly IStringLocalizer<GeneralTreeResource> _generalTreeStringLocalizer;

        public GeneralTreeManager(IGeneralTreeCodeGenerator generalTreeCodeGenerator,
            IGeneralTreeRepository<TTree, TPrimaryKey> generalTreeRepository,
            IOptions<GeneralTreeOptions> generalTreeOptionsAccess,
            IStringLocalizer<GeneralTreeResource> generalTreeStringLocalizer)
        {
            _generalTreeCodeGenerator = generalTreeCodeGenerator;
            _generalTreeRepository = generalTreeRepository;
            _generalTreeOptions = generalTreeOptionsAccess.Value;
            _generalTreeStringLocalizer = generalTreeStringLocalizer;
        }

        public virtual async Task CreateAsync(TTree tree, CancellationToken cancellationToken = default)
        {
            await _generalTreeRepository.InsertAsync(await GenerateTree(tree), cancellationToken: cancellationToken);
        }

        public virtual async Task BulkCreateAsync(TTree tree, Action<TTree> childrenAction = null, CancellationToken cancellationToken = default)
        {
            TraverseTree(await GenerateTree(tree), tree.Children, childrenAction);

            var allChildren = GetAllChildren(new []{ tree });

            var oldRootChildren = tree.Children;
            tree.Children = null;

            await _generalTreeRepository.InsertAsync(tree, true, cancellationToken);

            tree.Children = oldRootChildren;

            foreach (var c in allChildren)
            {
                var oldChildren = c.Children;
                c.Children = null;

                await _generalTreeRepository.InsertAsync(c, cancellationToken: cancellationToken);
                c.Children = oldChildren;

            }
        }

        public virtual async Task BulkCreateChildrenAsync(TTree parent, ICollection<TTree> children, Action<TTree> childrenAction = null, CancellationToken cancellationToken = default)
        {
            TraverseTree(parent, children, childrenAction);

            var alChildren = GetAllChildren(children, true);

            foreach (var c in alChildren)
            {
                var oldChildren = c.Children;
                c.Children = null;

                await _generalTreeRepository.InsertAsync(c, cancellationToken: cancellationToken);

                c.Children = oldChildren;
            }
        }

        public virtual async Task FillUpAsync(TTree tree, Action<TTree> childrenAction = null, CancellationToken cancellationToken = default)
        {
            TraverseTree(await GenerateTree(tree), tree.Children, childrenAction);
        }

        public virtual async Task UpdateNameAsync(TTree tree, Action<TTree> childrenAction = null, CancellationToken cancellationToken = default)
        {
            await CheckSameName(tree);

            var oldFullName = tree.FullName;

            if (tree.ParentId.HasValue)
            {
                var parent = await _generalTreeRepository.GetAsync(tree.ParentId.Value, cancellationToken: cancellationToken);
                tree.FullName = parent.FullName + _generalTreeOptions.Hyphen + tree.Name;
            }
            else
            {
                tree.FullName = tree.Name;
            }

            await _generalTreeRepository.UpdateAsync(tree, cancellationToken: cancellationToken);

            foreach (var child in await _generalTreeRepository.GetAllChildrenAsync(tree.Id, cancellationToken))
            {
                child.FullName = tree.FullName + _generalTreeOptions.Hyphen + _generalTreeCodeGenerator.RemoveParentCode(child.FullName, oldFullName);

                childrenAction?.Invoke(child);

                await _generalTreeRepository.UpdateAsync(child, cancellationToken: cancellationToken);
            }
        }

        public virtual async Task MoveAfterAsync(TPrimaryKey id, TPrimaryKey afterId, Action<TTree> childrenAction = null, CancellationToken cancellationToken = default)
        {
            var sourceTree = await _generalTreeRepository.GetAsync(id, cancellationToken: cancellationToken);
            var destTree = await _generalTreeRepository.GetAsync(afterId, cancellationToken: cancellationToken);

            var parent = destTree.ParentId.HasValue
                ? await _generalTreeRepository.GetAsync(destTree.ParentId.Value, cancellationToken: cancellationToken)
                : null;

            var oldChildren = await _generalTreeRepository.GetAllChildrenAsync(id, cancellationToken);
            var oldCode = sourceTree.Code;
            var oldFullName = sourceTree.FullName;

            sourceTree.Code = destTree.Code;
            sourceTree.Level = sourceTree.Code.Split('.').Length;
            sourceTree.ParentId = destTree.ParentId;
            sourceTree.FullName = parent != null
                ? parent.Name + _generalTreeOptions.Hyphen + sourceTree.Name
                : sourceTree.Name;

            await CheckSameName(sourceTree);

            var children = await _generalTreeRepository.GetChildrenAsync(parent?.Id, cancellationToken);

            var allNext = children.SkipWhile(x => x.Code != destTree.Code);

            await _generalTreeRepository.UpdateAsync(sourceTree, cancellationToken: cancellationToken);

            foreach (var child in oldChildren)
            {
                child.Code = _generalTreeCodeGenerator.MergeCode(sourceTree.Code, _generalTreeCodeGenerator.RemoveParentCode(child.Code, oldCode));
                child.FullName = _generalTreeCodeGenerator.MergeFullName(
                    sourceTree.FullName,
                    _generalTreeCodeGenerator.RemoveParentCode(child.FullName, oldFullName),
                    _generalTreeOptions.Hyphen);
                child.Level = child.Code.Split('.').Length;

                childrenAction?.Invoke(child);

                await _generalTreeRepository.UpdateAsync(child, cancellationToken: cancellationToken);
            }

            foreach (var child in allNext)
            {
                child.Code = _generalTreeCodeGenerator.GetNextCode(child.Code);

                childrenAction?.Invoke(child);

                await _generalTreeRepository.UpdateAsync(child, cancellationToken: cancellationToken);
            }
        }

        public async Task MoveToAsync(TPrimaryKey id, TPrimaryKey? parentId, Action<TTree> childrenAction = null, CancellationToken cancellationToken = default)
        {
            var tree = await _generalTreeRepository.GetAsync(id, cancellationToken: cancellationToken);
            if (tree.ParentId.Equals(parentId))
            {
                return;
            }

            var oldChildren = await _generalTreeRepository.GetAllChildrenAsync(id, cancellationToken);

            var oldCode = tree.Code;
            var oldFullName = tree.FullName;

            //Move Tree
            tree.Code = await GetNextCodeAsync(parentId, cancellationToken: cancellationToken);
            tree.Level = tree.Code.Split('.').Length;
            tree.ParentId = parentId;
            tree.FullName = await GetChildFullNameAsync(parentId, tree.Name, cancellationToken);
            await _generalTreeRepository.UpdateAsync(tree, cancellationToken: cancellationToken);

            await CheckSameName(tree);

            foreach (var child in oldChildren)
            {
                child.Code = _generalTreeCodeGenerator.MergeCode(tree.Code, _generalTreeCodeGenerator.RemoveParentCode(child.Code, oldCode));
                child.FullName = _generalTreeCodeGenerator.MergeFullName(
                    tree.FullName,
                    _generalTreeCodeGenerator.RemoveParentCode(child.FullName, oldFullName),
                    _generalTreeOptions.Hyphen);
                child.Level = child.Code.Split('.').Length;

                childrenAction?.Invoke(child);

                await _generalTreeRepository.UpdateAsync(child, cancellationToken: cancellationToken);
            }
        }

        public virtual async Task DeleteAsync(TPrimaryKey id, CancellationToken cancellationToken = default)
        {
            var tree = await _generalTreeRepository.GetAsync(id, cancellationToken: cancellationToken);
            if (tree != null)
            {
                await _generalTreeRepository.DeleteAsync(x => x.Code.StartsWith(tree.Code),
                    cancellationToken: cancellationToken);
            }
        }

        private async Task<TTree> GenerateTree(TTree tree)
        {
            tree.Code = await GetNextCodeAsync(tree.ParentId);
            tree.Level = tree.Code.Split('.').Length;

            if (tree.ParentId == null)
            {
                tree.FullName = tree.Name;
            }
            else
            {
                var parent = await _generalTreeRepository.GetAsync(tree.ParentId.Value);
                tree.FullName = parent.FullName + _generalTreeOptions.Hyphen + tree.Name;
            }

            await CheckSameName(tree);

            return tree;
        }

        private void TraverseTree(TTree parent, ICollection<TTree> children, Action<TTree> childrenAction = null)
        {
            if (children == null || !children.Any())
            {
                return;
            }

            var duplicateNames = children
                .GroupBy(x => x.Name)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .ToList();

            if (duplicateNames.Any())
            {
                throw new UserFriendlyException(_generalTreeStringLocalizer["GeneralTreeNameIsDuplicate", duplicateNames.First()]);
            }

            var index = 0;
            foreach (var c in children)
            {
                c.Code = _generalTreeCodeGenerator.MergeCode(parent.Code, _generalTreeCodeGenerator.CreateCode(++index));
                c.Level = c.Code.Split('.').Length;
                c.FullName = parent.FullName + _generalTreeOptions.Hyphen + c.Name;

                childrenAction?.Invoke(c);

                TraverseTree(c, c.Children, childrenAction);
            }
        }


        private static IEnumerable<TTree> GetAllChildren(IEnumerable<TTree> trees, bool includeSelf = false)
        {
            var allChildren = new List<TTree>();

            void Recursive(TTree t)
            {
                if (t.Children == null || !t.Children.Any())
                {
                    return;
                }

                foreach (var c in t.Children)
                {
                    allChildren.Add(c);
                    Recursive(c);
                }
            }

            foreach (var tree in trees)
            {
                if (includeSelf)
                {
                    allChildren.Add(tree);
                }

                Recursive(tree);
            }

            return allChildren.OrderBy(x => x.Code);
        }

        private async Task<string> GetNextCodeAsync(TPrimaryKey? parentId, TPrimaryKey? afterId = null, CancellationToken cancellationToken = default)
        {
            if (afterId == null)
            {
                var lastChildren = await _generalTreeRepository.GetLastChildrenAsync(parentId, cancellationToken);
                if (lastChildren != null)
                {
                    return _generalTreeCodeGenerator.GetNextCode(lastChildren.Code);
                }

                var parentCode = parentId != null
                    ? (await _generalTreeRepository.GetAsync(parentId.Value, cancellationToken: cancellationToken))?.Code
                    : null;

                return _generalTreeCodeGenerator.MergeCode(parentCode, _generalTreeCodeGenerator.CreateCode(1));
            }
            else
            {
                var afterTree = await _generalTreeRepository.GetAsync(afterId.Value, cancellationToken: cancellationToken);

                var parentCode = parentId != null
                    ? (await _generalTreeRepository.GetAsync(parentId.Value, cancellationToken: cancellationToken)).Code
                    : null;

                return _generalTreeCodeGenerator.MergeCode(parentCode, afterTree.Code);
            }
        }

        private async Task CheckSameName(TTree tree)
        {
            if (_generalTreeOptions.CheckSameNameExpression == null)
            {
                if (!await _generalTreeRepository.CheckSameNameAsync(tree.ParentId, tree.Name, tree.Id))
                {
                    return;
                }
            }
            else
            {
                var trees = (await _generalTreeRepository
                        .GetChildrenAsync(tree.ParentId))
                    .Where(x => x.Name == tree.Name)
                    .ToList();
                if (!trees.Any() || !trees.Any(x => _generalTreeOptions.CheckSameNameExpression(x, tree)))
                {
                    return;
                }
            }

            throw new UserFriendlyException(_generalTreeStringLocalizer["GeneralTreeNameIsDuplicate", tree.Name]);
        }

        private async Task<string> GetChildFullNameAsync(TPrimaryKey? parentId, string childFullName,
            CancellationToken cancellationToken = default)
        {
            if (!parentId.HasValue)
            {
                return childFullName;
            }

            var parent = await _generalTreeRepository.FindAsync(parentId.Value, cancellationToken: cancellationToken);
            return parent != null ? parent.FullName + _generalTreeOptions.Hyphen + childFullName : childFullName;
        }
    }
}
