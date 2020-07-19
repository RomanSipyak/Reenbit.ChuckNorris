using AutoMapper;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Moq;
using MoqExpression;
using Reenbit.ChuckNorris.DataAccess;
using Reenbit.ChuckNorris.DataAccess.Abstraction;
using Reenbit.ChuckNorris.DataAccess.Abstraction.Repositories;
using Reenbit.ChuckNorris.DataAccess.Repositories;
using Reenbit.ChuckNorris.Domain.DTOs.CategoryDTOS;
using Reenbit.ChuckNorris.Domain.DTOsProfiles;
using Reenbit.ChuckNorris.Domain.Entities;
using Reenbit.ChuckNorris.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace Reenbit.ChuckNorris.Tests
{
    public class CategoriesServiceShoud : IClassFixture<BaseMocks>
    {
        private readonly BaseMocks baseMocks;

        public CategoriesServiceShoud(BaseMocks baseMocks)
        {
            this.baseMocks = baseMocks;
        }

        [Theory]
        [InlineData("Category1")]
        [InlineData("Category2")]
        public async Task CreateCategory(string categoryTitle)
        {
            //Arrange
            CreateCategoryDTO categoryDTO = new CreateCategoryDTO { Title = categoryTitle };
            CategoryDTO ctegoryDTO = new CategoryDTO { Id = 0, Title = categoryTitle };
            var categoriesExisting = new Dictionary<string, bool> { { "Category1", false }, { "Category2", true } };
            //mocks and setups
            var existCategory = categoriesExisting[categoryDTO.Title];
            baseMocks.categoryRepositoryMock.Setup(_ => _.AnyAsync(It.IsAny<Expression<Func<Category, bool>>>())).ReturnsAsync(existCategory);
            baseMocks.unitOfWorkMock.Setup(_ => _.GetRepository<ICategoryRepository>()).Returns(baseMocks.categoryRepositoryMock.Object);
            baseMocks.unitOfWorkFactoryMock.Setup(_ => _.CreateUnitOfWork()).Returns(baseMocks.unitOfWorkMock.Object);
            //mocks and setups
            var categoryservice = new CategoryService(baseMocks.unitOfWorkFactoryMock.Object, baseMocks.mapper);
            //Arrange
            //Assert
            if (categoryTitle.Equals("Category1"))
                ctegoryDTO.Should().BeEquivalentTo(categoryservice.CreateCategoryAsync(categoryDTO).Result);
            if (categoryTitle.Equals("Category2"))
                Assert.Equal($"Category with {categoryDTO.Title} already exist",
                            (await Assert.ThrowsAsync<ArgumentException>(() => categoryservice.CreateCategoryAsync(categoryDTO))).Message);
            //Assert
        }

        [Fact]
        public void GetAllCategories()
        {
            //Arrange
            Func<Expression, Expression, bool> eq = ExpressionEqualityComparer.Instance.Equals;
            var categories = new List<string> {"Category1",
                                               "Category2",
                                               "Category3",
                                               "Category4"};

            //mocks and setups
            Expression<Func<Category, string>> titleSelector = c => c.Title;
            baseMocks.categoryRepositoryMock.Setup(_ => _.FindAndMapAsync(
                It.Is<Expression<Func<Category, string>>>(x => eq(x, titleSelector)),
                It.Is<Expression<Func<Category, bool>>>(x => x == null),
                It.Is<Func<IQueryable<Category>, IOrderedQueryable<Category>>>(x => x == null),
                It.Is<IEnumerable<Expression<Func<Category, object>>>>(x => x == null))).ReturnsAsync(categories);

            baseMocks.unitOfWorkMock.Setup(_ => _.GetRepository<ICategoryRepository>()).Returns(baseMocks.categoryRepositoryMock.Object);
            baseMocks.unitOfWorkFactoryMock.Setup(_ => _.CreateUnitOfWork()).Returns(baseMocks.unitOfWorkMock.Object);
            //mocks and setups
            var categoryservice = new CategoryService(baseMocks.unitOfWorkFactoryMock.Object, baseMocks.mapper);
            //Arrange
            //Act
            var categoriesResult = categoryservice.GetAllCategoriesAsync().Result;
            //Act
            Assert.Equal(categories, categoriesResult);
        }
    }
}
