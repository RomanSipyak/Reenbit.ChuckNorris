using AutoMapper;
using Reenbit.ChuckNorris.Domain.DTOs.JokeDTOS;
using Reenbit.ChuckNorris.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reenbit.ChuckNorris.Domain.DTOsProfiles
{
    public class MappingProfileForDTOs : Profile
    {
        public MappingProfileForDTOs()
        {
            CreateMap<Joke, JokeDTO>().ReverseMap();
            CreateMap<CreateJokeDTO, Joke>();
        }
    }
}
