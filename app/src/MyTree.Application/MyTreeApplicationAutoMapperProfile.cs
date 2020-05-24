using AutoMapper;

namespace MyTree
{
    public class MyTreeApplicationAutoMapperProfile : Profile
    {
        public MyTreeApplicationAutoMapperProfile()
        {
            /* You can configure your AutoMapper mapping configuration here.
             * Alternatively, you can split your mapping configurations
             * into multiple profile classes for a better organization. */

            CreateMap<Region, RegionDto>();
        }
    }
}
