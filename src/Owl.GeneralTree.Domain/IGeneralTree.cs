using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace Owl.GeneralTree
{
    public interface IGeneralTree<TTree, TPrimaryKey> : IEntity<TPrimaryKey>
        where TPrimaryKey : struct
    {
        string Name { get; set; }

        string FullName { get; set; }

        string Code { get; set; }

        int Level { get; set; }

        TTree Parent { get; set; }

        TPrimaryKey? ParentId { get; set; }

        ICollection<TTree> Children { get; set; }
    }
}
