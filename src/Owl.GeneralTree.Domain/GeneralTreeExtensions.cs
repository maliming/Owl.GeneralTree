using System;
using System.Collections.Generic;
using System.Linq;

namespace Owl.GeneralTree
{
    public static class GeneralTreeExtensions
    {
        public static IEnumerable<TTree> ToTree<TTree, TPrimaryKey>(this IEnumerable<TTree> tree)
            where TPrimaryKey : struct
            where TTree : class, IGeneralTree<TTree, TPrimaryKey>
        {
            var treeDic = tree.ToDictionary(x => x.Id);

            foreach (var t in treeDic.Where(x => x.Value.ParentId.HasValue).Select(x => x.Value))
            {
                // ReSharper disable once PossibleInvalidOperationException
                if (!treeDic.ContainsKey(t.ParentId.Value))
                {
                    continue;
                }

                var parent = treeDic[t.ParentId.Value];
                parent.Children ??= new List<TTree>();

                parent.Children.Add(t);
            }

            if (treeDic.Values.Any(x => x.ParentId == null))
            {
                return treeDic.Values.Where(x => x.ParentId == null);
            }

            return treeDic.Values.Where(x =>
                x.ParentId != null && !treeDic.Values.Select(q => q.Id).Contains(x.ParentId.Value));
        }

        public static IEnumerable<TTree> ToTreeOrderBy<TTree, TPrimaryKey, TTreeProperty>(this IEnumerable<TTree> tree,
            Func<TTree, TTreeProperty> propertySelector)
            where TPrimaryKey : struct
            where TTree : class, IGeneralTree<TTree, TPrimaryKey>
        {
            var treeDic = tree.ToDictionary(x => x.Id);

            foreach (var t in treeDic.Where(x => x.Value.ParentId.HasValue).Select(x => x.Value))
            {
                // ReSharper disable once PossibleInvalidOperationException
                if (!treeDic.ContainsKey(t.ParentId.Value))
                {
                    continue;
                }

                var parent = treeDic[t.ParentId.Value];
                parent.Children ??= new List<TTree>();

                parent.Children.Add(t);
            }

            foreach (var t in treeDic.Where(x => !x.Value.Children.IsNullOrEmpty()).Select(x => x.Value))
            {
                t.Children = t.Children.OrderBy(propertySelector).ToList();
            }

            if (treeDic.Values.Any(x => x.ParentId == null))
            {
                return treeDic.Values.Where(x => x.ParentId == null).OrderBy(propertySelector);
            }

            return treeDic.Values.Where(x =>
                x.ParentId != null && !treeDic.Values.Select(q => q.Id).Contains(x.ParentId.Value));
        }

        public static IEnumerable<TTree> ToTreeOrderByDescending<TTree, TPrimaryKey, TTreeProperty>(
            this IEnumerable<TTree> tree, Func<TTree, TTreeProperty> propertySelector)
            where TPrimaryKey : struct
            where TTree : class, IGeneralTree<TTree, TPrimaryKey>
        {
            var treeDic = tree.ToDictionary(x => x.Id);

            foreach (var t in treeDic.Where(x => x.Value.ParentId.HasValue).Select(x => x.Value))
            {
                // ReSharper disable once PossibleInvalidOperationException
                if (!treeDic.ContainsKey(t.ParentId.Value))
                {
                    continue;
                }

                var parent = treeDic[t.ParentId.Value];
                parent.Children ??= new List<TTree>();

                parent.Children.Add(t);
            }

            foreach (var t in treeDic.Where(x => !x.Value.Children.IsNullOrEmpty()).Select(x => x.Value))
            {
                t.Children = t.Children.OrderByDescending(propertySelector).ToList();
            }

            if (treeDic.Values.Any(x => x.ParentId == null))
            {
                return treeDic.Values.Where(x => x.ParentId == null).OrderByDescending(propertySelector);
            }

            return treeDic.Values.Where(x => x.ParentId != null && !treeDic.Values.Select(q => q.Id).Contains(x.ParentId.Value));
        }

    }
}
