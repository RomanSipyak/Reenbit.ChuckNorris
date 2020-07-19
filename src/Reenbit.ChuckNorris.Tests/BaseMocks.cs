using AutoMapper;
using Moq;
using Reenbit.ChuckNorris.DataAccess.Abstraction;
using Reenbit.ChuckNorris.DataAccess.Abstraction.Repositories;
using Reenbit.ChuckNorris.Domain.DTOsProfiles;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reenbit.ChuckNorris.Tests
{
    public class BaseMocks
    {
        public readonly Mock<IUnitOfWorkFactory> unitOfWorkFactoryMock;
        public readonly Mock<IUnitOfWork> unitOfWorkMock;
        public readonly Mock<ICategoryRepository> categoryRepositoryMock;
        public readonly Mock<IJokeRepository> jokeRepositoryMock;
        public readonly IMapper mapper;

        public BaseMocks()
        {
            this.unitOfWorkFactoryMock = new Mock<IUnitOfWorkFactory>();
            this.unitOfWorkMock = new Mock<IUnitOfWork>();
            this.categoryRepositoryMock = new Mock<ICategoryRepository>();
            this.jokeRepositoryMock = new Mock<IJokeRepository>();
            MapperConfiguration mappingConfig = new MapperConfiguration(mc => mc.AddProfile(new MappingProfileForDTOs()));
            this.mapper = mappingConfig.CreateMapper();
        }
    }
}
