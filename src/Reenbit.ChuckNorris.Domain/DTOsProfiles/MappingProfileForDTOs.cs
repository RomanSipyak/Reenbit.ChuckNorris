using AutoMapper;
using Reenbit.ChuckNorris.Domain.DTOs.JokeDTOS;
using Reenbit.ChuckNorris.Domain.DTOs.UserDTOS;
using Reenbit.ChuckNorris.Domain.Entities;

namespace Reenbit.ChuckNorris.Domain.DTOsProfiles
{
    public class MappingProfileForDTOs : Profile
    {
        public MappingProfileForDTOs()
        {
            CreateMap<Joke, JokeDto>().ReverseMap();
            CreateMap<CreateJokeDto, Joke>();
            CreateMap<UserRegisterDto, User>().ForMember(desc => desc.UserName, opt => opt.MapFrom(src => src.Email));
            CreateMap<SignInUserDto, UserDto>().ForMember(desc => desc.Email, opt => opt.MapFrom(src => src.User.Email))
                                               .ForMember(desc => desc.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
                                               .ForMember(desc => desc.LastName, opt => opt.MapFrom(src => src.User.LastName))
                                               .ForMember(desc => desc.Id, opt => opt.MapFrom(src => src.User.Id))
                                               .ForMember(desc => desc.Roles, opt => opt.MapFrom(src => src.Roles));
        }
    }
}
