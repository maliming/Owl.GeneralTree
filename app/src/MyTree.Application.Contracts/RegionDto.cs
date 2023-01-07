using System;
using System.Collections.Generic;
using Owl.GeneralTree;
using Volo.Abp.Application.Dtos;

namespace MyTree;

public class RegionDto : EntityDto<Guid>, IGeneralTreeDto<RegionDto, Guid>
{
    public RegionDto()
    {

    }

    public RegionDto(Guid id)
    {
        Id = id;
    }

    public string Name { get; set; }

    public string FullName { get; set; }

    public string Code { get; set; }

    public int Level { get; set; }

    public Guid? ParentId { get; set; }

    public ICollection<RegionDto> Children { get; set; }
}