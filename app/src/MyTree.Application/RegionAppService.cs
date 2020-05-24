using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Owl.GeneralTree;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;

namespace MyTree
{
    public class RegionAppService : MyTreeAppService, IRegionAppService
    {
        private readonly IGeneralTreeManager<Region, Guid> _generalTreeManager;
        private readonly IRepository<Region, Guid> _generalRepository;
        private readonly RegionDataSeedContributor _regionDataSeedContributor;

        public RegionAppService(IGeneralTreeManager<Region, Guid> generalTreeManager, IRepository<Region, Guid> generalRepository, RegionDataSeedContributor regionDataSeedContributor)
        {
            _generalTreeManager = generalTreeManager;
            _generalRepository = generalRepository;
            _regionDataSeedContributor = regionDataSeedContributor;
        }

        public async Task CreateAsync(CreateDto input)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(UpdateInput input)
        {
            throw new NotImplementedException();
        }

        public async Task<RegionDto> GetAsync(Guid id)
        {
            var region =  await _generalRepository.GetAsync(id);
            return ObjectMapper.Map<Region, RegionDto>(region);
        }

        public async Task<List<RegionDto>> GetTreesAsync(GetTreesDto input)
        {
            List<Region> regions;
            if (input.ParentId != null)
            {
                var parent = await _generalRepository.GetAsync(input.ParentId.Value);
                regions = await _generalRepository.Where(x => x.Code.StartsWith(parent.Code))
                    .ToListAsync();
            }
            else
            {
                regions = await _generalRepository.GetListAsync();
            }

            var regionDtos = ObjectMapper.Map<List<Region>, List<RegionDto>>(regions);
            return regionDtos.ToTreeOrderBy<RegionDto, Guid, string>(x => x.Code).ToList();
        }

        public async Task MoveToBeforeAsync(Guid id, Guid beforeId)
        {
            await _generalTreeManager.MoveToBeforeAsync(id, beforeId);
        }

        public async Task MoveToAsync(MoveToDto input)
        {
            await _generalTreeManager.MoveToAsync(input.Id, input.ParentId);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _generalTreeManager.DeleteAsync(id);
        }

        public async Task RegenerateAsync(RegenerateDto input)
        {
            await _generalTreeManager.RegenerateAsync(input.ParentId);
        }

        public async Task ReseedAsync()
        {
            await _generalRepository.DeleteAsync(x => true, true);

            await _regionDataSeedContributor.SeedAsync(new DataSeedContext());
        }
    }
}
