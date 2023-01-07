using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace MyTree;

public interface IRegionAppService : IApplicationService
{
    Task CreateAsync(CreateDto input);

    Task UpdateAsync(UpdateInput input);

    Task<RegionDto> GetAsync(Guid id);

    Task<List<RegionDto>> GetTreesAsync(GetTreesDto input);

    Task MoveToBeforeAsync(Guid id, Guid beforeId);

    Task MoveToAsync(MoveToDto input);

    Task DeleteAsync(Guid id);

    Task RegenerateAsync(RegenerateDto input);

    Task ReseedAsync();
}