using AutoMapper;
using Entities.Dtos;
using Entities.Models;

namespace CompanyEmployees.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Company, CompanyDto>()
                .ForMember(cDto => cDto.FullAddress, opt => opt.MapFrom(x => string.Join(x.Address, x.Country)));
        }
    }
}