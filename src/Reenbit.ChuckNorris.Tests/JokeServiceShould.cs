using AutoMapper;
using FluentAssertions;
using Microsoft.VisualBasic;
using Moq;
using Reenbit.ChuckNorris.DataAccess.Abstraction;
using Reenbit.ChuckNorris.DataAccess.Abstraction.Repositories;
using Reenbit.ChuckNorris.Domain.DTOs.JokeDTOS;
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
    [Collection("ShareCollection")]
    public class JokeServiceShould
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
            //Assert
            Assert.Null(await jokeService.GetRandomJokeAsync(null));
            //Assert
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
            //Assert
            Assert.Equal("search.query: size must be between 3 and 120", (await Assert.ThrowsAsync<ArgumentException>(() => jokeService.SearchJokesAsync(invalidSearch))).Message);
            //Assert
        }

        [Fact]
        [Trait("GetAllJokes", "ValidData")]
        public void GetAllJokes()
        {
            //Arrange
            var jokesDtos = new List<JokeDto>
            {
                new JokeDto
                {
                    Id = 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow.AddDays(5),
                    Value = "Joke1",
                    Categories = new List<string> {"Category1", "Category2"}
                },
                new JokeDto
                {
                    Id = 2,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow.AddDays(5),
                    Value = "Joke2",
                    Categories = new List<string> {"Category1"}
                }
            };

            //mocks and setups
            Expression<Func<Category, string>> titleSelector = c => c.Title;
            baseMocks.jokeRepositoryMock.Setup(_ => _.FindAndMapAsync(
                It.IsAny<Expression<Func<Joke, JokeDto>>>(),
                It.Is<Expression<Func<Joke, bool>>>(x => x == null),
                It.Is<Func<IQueryable<Joke>, IOrderedQueryable<Joke>>>(x => x == null),
                It.Is<IEnumerable<Expression<Func<Joke, object>>>>(x => x == null))).ReturnsAsync(jokesDtos);

            baseMocks.unitOfWorkMock.Setup(_ => _.GetRepository<IJokeRepository>()).Returns(baseMocks.jokeRepositoryMock.Object);
            baseMocks.unitOfWorkFactoryMock.Setup(_ => _.CreateUnitOfWork()).Returns(baseMocks.unitOfWorkMock.Object);
            //mocks and setups
            var jokeService = new JokeService(baseMocks.unitOfWorkFactoryMock.Object, baseMocks.mapper);
            //Arrange
            //Act
            var jokesResult = jokeService.GetAllJokesAsync().Result;
            //Act
            //Assert
            Assert.Equal(jokesDtos, jokesResult);
            //Assert
        }

        [Fact]
        [Trait("GetJokeAsync", "ValidData")]
        public void GetJokeAsync()
        {
            //Arrange
            var jokeDto =
                new JokeDto
                {
                    Id = 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow.AddDays(5),
                    Value = "Joke1",
                    Categories = new List<string> { "Category1", "Category2" }
                };


            //mocks and setups
            Expression<Func<Category, string>> titleSelector = c => c.Title;
            baseMocks.jokeRepositoryMock.Setup(_ => _.FindByKeyAndMapAsync(
                                                    It.IsAny<Expression<Func<Joke, bool>>>(),
                                                    It.IsAny<Expression<Func<Joke, JokeDto>>>())).ReturnsAsync(jokeDto);

            baseMocks.unitOfWorkMock.Setup(_ => _.GetRepository<IJokeRepository>()).Returns(baseMocks.jokeRepositoryMock.Object);
            baseMocks.unitOfWorkFactoryMock.Setup(_ => _.CreateUnitOfWork()).Returns(baseMocks.unitOfWorkMock.Object);
            //mocks and setups
            var jokeService = new JokeService(baseMocks.unitOfWorkFactoryMock.Object, baseMocks.mapper);
            //Arrange
            //Act
            var jokeResult = jokeService.GetJokeAsync(It.IsAny<int>()).Result;
            //Act
            //Assert
            Assert.Equal(jokeDto, jokeResult);
            //Assert
        }

        [Fact]
        [Trait("CreateNewJokeAsync", "ValidData")]
        public void CreateNewJokeAsync()
        {
            //Arrange
            var createJokeDto = new CreateJokeDto
            {
                Value = "Joke1",
                Categories = new List<int> { 1, 2 }
            };

            var categoryCollection = new List<Category>
            {
                new Category
                {
                    Id = 1,
                    Title = "Category1"
                },
                new Category
                {
                    Id = 2,
                    Title = "Category2"
                }
            };

            var expectedJoke = new JokeDto
            {
                Id = 0,
                Value = "Joke1",
                Categories = new List<string> { "Category1", "Category2" }
            };

            //mocks and setups
            Expression<Func<Category, string>> titleSelector = c => c.Title;
            baseMocks.categoryRepositoryMock.Setup(_ => _.Find(It.IsAny<Expression<Func<Category, bool>>>(),
                                                               It.Is<Func<IQueryable<Category>, IOrderedQueryable<Category>>>(x => x == null),
                                                               It.Is<IEnumerable<Expression<Func<Category, object>>>>(x => x == null))).Returns(categoryCollection);

            baseMocks.unitOfWorkMock.Setup(_ => _.GetRepository<IJokeRepository>()).Returns(baseMocks.jokeRepositoryMock.Object);
            baseMocks.unitOfWorkMock.Setup(_ => _.GetRepository<ICategoryRepository>()).Returns(baseMocks.categoryRepositoryMock.Object);
            baseMocks.unitOfWorkFactoryMock.Setup(_ => _.CreateUnitOfWork()).Returns(baseMocks.unitOfWorkMock.Object);
            //mocks and setups
            var jokeService = new JokeService(baseMocks.unitOfWorkFactoryMock.Object, baseMocks.mapper);
            //Arrange
            //Act
            var jokeResult = jokeService.CreateNewJokeAsync(createJokeDto).Result;
            //Act
            //Assert
            expectedJoke.Should().BeEquivalentTo(jokeResult);
            //Assert
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
