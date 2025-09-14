using Application.Dtos;
using Application.Interfaces;
using Application.Services;
using Domain.Enums;
using Domain.Models;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Test.ArticleTests
{
    public class ArticleServiceTests
    {
        private readonly IGenericRepository<Article> fakeRepository;
        private readonly ILogger<ArticleService> fakeLogger;

        public ArticleServiceTests()
        {
            fakeRepository = A.Fake<IGenericRepository<Article>>();
            fakeLogger = A.Fake<ILogger<ArticleService>>();
        }

        [Fact]
        [Trait("Category", "ArticleService")]
        public async Task CreateArticle_ShouldReturnArticle_WhenValidInput()
        {
            // Arrange
            var articleService = new ArticleService(fakeRepository, fakeLogger);
            
            var newArticle = new CreateArticleDto
            {
                MaterialType = "Munskydd",
                Amount = 10,
                FullAmount = 10,
                IsOrdered = false,
                Unit = Unit.st
            };

            A.CallTo(() => fakeRepository.FindAsync(A<Expression<Func<Article, bool>>>.Ignored))
             .Returns(Task.FromResult(new List<Article>()));

            A.CallTo(() => fakeRepository.AddAsync(A<Article>.Ignored))
                .ReturnsLazily(call => Task.FromResult(call.GetArgument<Article>(0)));

            var result = await articleService.CreateArticle(newArticle);

            Assert.False(result.IsFailure);
            Assert.NotNull(result.Data);
            Assert.IsType<Article>(result.Data);
            Assert.IsType<string>(result.Data!.Id);
            Assert.Equal("Munskydd", result.Data.MaterialType);
            Assert.Equal(10, result.Data.Amount);
            Assert.Equal(10, result.Data.FullAmount);
            Assert.False(result.Data.IsOrdered);
        }

        [Fact]
        [Trait("Category", "ArticleService")]
        public async Task CreateArticle_ShouldReturnError_WhenArticleAlreadyExists()
        {
            // Arrange
            var articleService = new ArticleService(fakeRepository, fakeLogger);
            var existingArticles = new List<Article>
            {
                new() {
                    Id = "existing-id",
                    MaterialType = "Munskydd",
                    Amount = 10,
                    FullAmount = 10,
                    IsOrdered = false,
                    Unit = Unit.st
                }
            };

            A.CallTo(() => fakeRepository.FindAsync(A<Expression<Func<Article, bool>>>.Ignored))
             .Returns(Task.FromResult(existingArticles));
           
            var newArticle = new CreateArticleDto
            {
                MaterialType = "Munskydd",
                Amount = 5,
                FullAmount = 5,
                IsOrdered = false,
                Unit = Unit.st
            };

            var result = await articleService.CreateArticle(newArticle);
            
            Assert.True(result.IsFailure);
            Assert.Null(result.Data);
            Assert.Equal("An article with the same material type already exists.", result.ErrorMessage);
        }
    }
}
