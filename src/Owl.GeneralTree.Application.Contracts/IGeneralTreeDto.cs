using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Owl.GeneralTree;

public interface IGeneralTreeDto<TTree, TPrimaryKey> : IEntityDto<TPrimaryKey>
    where TPrimaryKey : struct
{
    TPrimaryKey? ParentId { get; set; }

    ICollection<TTree> Children { get; set; }
}