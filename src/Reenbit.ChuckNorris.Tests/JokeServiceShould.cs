using AutoMapper;
using Moq;
using Reenbit.ChuckNorris.DataAccess.Abstraction;
using Reenbit.ChuckNorris.DataAccess.Abstraction.Repositories;
using Reenbit.ChuckNorris.Domain.DTOsProfiles;
using Reenbit.ChuckNorris.Domain.Entities;
using Reenbit.ChuckNorris.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Reenbit.ChuckNorris.Tests
{
    public class JokeServiceShould : IClassFixture<BaseMocks>
    {
        private readonly BaseMocks baseMocks;

        public JokeServiceShould(BaseMocks baseMocks)
        {
            this.baseMocks = baseMocks;
        }


        [Fact]
        [Trait("GetRandomJokeAsync", "InvalidData")]
        public async System.Threading.Tasks.Task ThrowExceptionAsyncForInvalidCategory()
        {
            //Arrange
            //mocks and setups
            baseMocks.categoryRepositoryMock.Setup(_ => _.AnyAsync(It.IsAny<Expression<Func<Category, bool>>>())).ReturnsAsync(false);
            baseMocks.unitOfWorkMock.Setup(_ => _.GetRepository<ICategoryRepository>()).Returns(baseMocks.categoryRepositoryMock.Object);
            baseMocks.unitOfWorkMock.Setup(_ => _.GetRepository<IJokeRepository>()).Returns(baseMocks.jokeRepositoryMock.Object);
            baseMocks.unitOfWorkFactoryMock.Setup(_ => _.CreateUnitOfWork()).Returns(baseMocks.unitOfWorkMock.Object);
            //mocks and setups
            var jokeService = new JokeService(baseMocks.unitOfWorkFactoryMock.Object, baseMocks.mapper);
            //Arrange
            Assert.Equal($"No jokes for category \"TestCategory\" found.", (await Assert.ThrowsAsync<ArgumentException>(() => jokeService.GetRandomJokeAsync("TestCategory"))).Message);
        }

        /* [Fact]
         public async System.Threading.Tasks.Task GetRandomJokeThrowExceptionAsync2()
         {
          */   /*//Arrange
             //mocks and setups
             var randomeIds = new List<int> { 1, 2, 3, 4 };
             var unitOfWorkFactoryMock = new Mock<IUnitOfWorkFactory>();
             var unitOfWorkMock = new Mock<IUnitOfWork>();
             var categoryRepositoryMock = new Mock<ICategoryRepository>();
             var jokeRepositoryMock = new Mock<IJokeRepository>();
             categoryRepositoryMock.Setup(_ => _.AnyAsync(It.IsAny<Expression<Func<Category, bool>>>())).ReturnsAsync(false);
             jokeRepositoryMock.Setup(_ => _.FindAndMapAsync(It.IsAny<Expression<Func<Joke, int>>>(),
                                                                 It.IsAny<Expression<Func<Joke, bool>>>(),
                                                                 null,
                                                                 null)).ReturnsAsync(randomeIds);
             unitOfWorkMock.Setup(_ => _.GetRepository<ICategoryRepository>()).Returns(categoryRepositoryMock.Object);
             unitOfWorkMock.Setup(_ => _.GetRepository<IJokeRepository>()).Returns(jokeRepositoryMock.Object);
             unitOfWorkFactoryMock.Setup(_ => _.CreateUnitOfWork()).Returns(unitOfWorkMock.Object);
             //mocks and setups
             MapperConfiguration mappingConfig = new MapperConfiguration(mc => mc.AddProfile(new MappingProfileForDTOs()));
             var _mapper = mappingConfig.CreateMapper();
             var jokeService = new JokeService(unitOfWorkFactoryMock.Object, _mapper);
             //Arrange
             Assert.Equal($"No jokes for category \"TestCategory\" found.", (await Assert.ThrowsAsync<ArgumentException>(() => jokeService.GetRandomJokeAsync("TestCategory"))).Message);*/
        /* }*/

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        [Trait("GetRandomJokeAsync", "ValidData")]
        public async System.Threading.Tasks.Task ReturnNull(bool categoryExist)
        {
            //Arrange
            //mocks and setups
            var randomeIds = new List<int> { };
            baseMocks.categoryRepositoryMock.Setup(_ => _.AnyAsync(It.IsAny<Expression<Func<Category, bool>>>())).ReturnsAsync(categoryExist);
            baseMocks.jokeRepositoryMock.Setup(_ => _.FindAndMapAsync(It.IsAny<Expression<Func<Joke, int>>>(),
                                                            It.IsAny<Expression<Func<Joke, bool>>>(),
                                                            null,
                                                            null)).ReturnsAsync(randomeIds);
            baseMocks.unitOfWorkMock.Setup(_ => _.GetRepository<ICategoryRepository>()).Returns(baseMocks.categoryRepositoryMock.Object);
            baseMocks.unitOfWorkMock.Setup(_ => _.GetRepository<IJokeRepository>()).Returns(baseMocks.jokeRepositoryMock.Object);
            baseMocks.unitOfWorkFactoryMock.Setup(_ => _.CreateUnitOfWork()).Returns(baseMocks.unitOfWorkMock.Object);
            //mocks and setups
            var jokeService = new JokeService(baseMocks.unitOfWorkFactoryMock.Object, baseMocks.mapper);
            //Arrange
            Assert.Null(await jokeService.GetRandomJokeAsync(null));
        }

        [Theory, ClassData(typeof(InvalidSearchData))]
        [Trait("SearchJokesAsync", "InvalidData")]
        public async System.Threading.Tasks.Task SearchJokesAsyncThrowException(string invalidSearch)
        {
            //Arrange
            //mocks and setups
            var randomeIds = new List<int> { };
            baseMocks.unitOfWorkFactoryMock.Setup(_ => _.CreateUnitOfWork()).Returns(baseMocks.unitOfWorkMock.Object);
            //mocks and setups
            var jokeService = new JokeService(baseMocks.unitOfWorkFactoryMock.Object, baseMocks.mapper);
            //Arrange
            Assert.Equal("search.query: size must be between 3 and 120", (await Assert.ThrowsAsync<ArgumentException>(() => jokeService.SearchJokesAsync(invalidSearch))).Message);
        }
    }

    public class InvalidSearchData : IEnumerable<object[]>
    {
        private readonly List<object[]> _data = new List<object[]>
        {
            new object[] { "he" },
            new object[] { "    " },
            new object[] { null },
            new object[] { new string('x', 300) }
        };

        public IEnumerator<object[]> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
