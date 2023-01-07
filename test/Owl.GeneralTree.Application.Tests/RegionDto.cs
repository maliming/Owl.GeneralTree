using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Owl.GeneralTree;

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

    public Guid? ParentId { get; set; }

    public ICollection<RegionDto> Children { get; set; }
}