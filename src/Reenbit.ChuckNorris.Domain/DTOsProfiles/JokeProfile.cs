using AutoMapper;
using Reenbit.ChuckNorris.Domain.DTOs;
using Reenbit.ChuckNorris.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reenbit.ChuckNorris.Domain.DTOsProfiles
{
    public class JokeProfile : Profile
    {
        public JokeProfile()
        {
            CreateMap<Joke, JokeDTO>().ReverseMap();
        }
    }
}
