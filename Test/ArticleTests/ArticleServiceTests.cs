using Application.Dtos;
using Application.Interfaces;
using Application.Services;
using Domain.Enums;
using Domain.Models;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

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
        [Trait("Create", "ArticleService")]
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
        [Trait("Create", "ArticleService")]
        public async Task CreateArticle_ShouldReturnError_WhenArticleAlreadyExists()
        {
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
            Assert.Equal($"Article with MaterialType {newArticle.MaterialType} already exists.", result.ErrorMessage);
        }

        [Fact]
        [Trait("Create", "ArticleService")]
        public async Task CreateArticle_ShouldReturnError_WhenAmountIsNegative()
        {
            var articleService = new ArticleService(fakeRepository, fakeLogger);
            var newArticle = new CreateArticleDto
            {
                MaterialType = "Handskar",
                Amount = -5,
                FullAmount = 10,
                IsOrdered = false,
                Unit = Unit.st
            };

            var result = await articleService.CreateArticle(newArticle);

            Assert.True(result.IsFailure);
            Assert.Null(result.Data);
            Assert.Equal("Amount must be zero or greater.", result.ErrorMessage);
        }

        [Fact]
        [Trait("Create", "ArticleService")]
        public async Task CreateArticle_ShouldReturnError_WhenFullAmountIsNegative()
        {
            var articleService = new ArticleService(fakeRepository, fakeLogger);
            var newArticle = new CreateArticleDto
            {
                MaterialType = "Handskar",
                Amount = 5,
                FullAmount = -10,
                IsOrdered = false,
                Unit = Unit.st
            };

            var result = await articleService.CreateArticle(newArticle);

            Assert.True(result.IsFailure);
            Assert.Null(result.Data);
            Assert.Equal("FullAmount must be zero or greater.", result.ErrorMessage);
        }

        [Fact]
        [Trait("Create", "ArticleService")]
        public async Task CreateArticle_ShouldReturnError_WhenFullAmountLessThanAmount()
        {
            var articleService = new ArticleService(fakeRepository, fakeLogger);
            var newArticle = new CreateArticleDto
            {
                MaterialType = "Handskar",
                Amount = 15,
                FullAmount = 10,
                IsOrdered = false,
                Unit = Unit.st
            };

            var result = await articleService.CreateArticle(newArticle);

            Assert.True(result.IsFailure);
            Assert.Null(result.Data);
            Assert.Equal("Amount cannot exceed FullAmount.", result.ErrorMessage);
        }

        [Fact]
        [Trait("Create", "ArticleService")]
        public async Task CreateArticle_ShouldReturnError_WhenMaterialTypeIsEmpty()
        {
            var articleService = new ArticleService(fakeRepository, fakeLogger);
            var newArticle = new CreateArticleDto
            {
                MaterialType = "",
                Amount = 5,
                FullAmount = 10,
                IsOrdered = false,
                Unit = Unit.st
            };

            var result = await articleService.CreateArticle(newArticle);

            Assert.True(result.IsFailure);
            Assert.Null(result.Data);
            Assert.Equal("MaterialType is required.", result.ErrorMessage);
        }

        [Fact]
        [Trait("Create", "ArticleService")]
        public async Task CreateArticle_ShouldReturnCorrectStatus_BasedOnAmountAndFullAmount()
        {
            var articleService = new ArticleService(fakeRepository, fakeLogger);
            var newArticleFullt = new CreateArticleDto
            {
                MaterialType = "Artikel Fullt",
                Amount = 10,
                FullAmount = 10,
                IsOrdered = false,
                Unit = Unit.st
            };

            var resultFullt = await articleService.CreateArticle(newArticleFullt);

            Assert.False(resultFullt.IsFailure);
            Assert.Equal(ArticleStatus.Fullt, resultFullt.Data!.Status);
        }

        [Fact]
        [Trait("Create", "ArticleService")]
        public async Task CreateArticle_ShouldReturnMellanStatus_WhenAmountLessThanFullAmount()
        {
            var articleService = new ArticleService(fakeRepository, fakeLogger);
            var newArticleNotFullt = new CreateArticleDto
            {
                MaterialType = "Artikel Not Fullt",
                Amount = 5,
                FullAmount = 10,
                IsOrdered = false,
                Unit = Unit.st
            };

            A.CallTo(() => fakeRepository.AddAsync(A<Article>.Ignored))
                .ReturnsLazily(call => Task.FromResult(call.GetArgument<Article>(0)));

            var resultMellan = await articleService.CreateArticle(newArticleNotFullt);

            Assert.False(resultMellan.IsFailure);
            Assert.Equal(ArticleStatus.Mellan, resultMellan.Data!.Status);
        }

        [Fact]
        [Trait("Create", "ArticleService")]
        public async Task CreateArticle_ShouldReturnKritiskStatus_WhenAmountIsCloseToZero()
        {
            var articleService = new ArticleService(fakeRepository, fakeLogger);

            var existingArticle = new Article
            {
                Id = "1",
                MaterialType = "Munskydd",
                Amount = 1,
                FullAmount = 10,
                IsOrdered = false,
                Unit = Unit.st
            };

            A.CallTo(() => fakeRepository.AddAsync(A<Article>.Ignored))
                .ReturnsLazily(call => Task.FromResult(call.GetArgument<Article>(0)));

            var newArticleGod = new CreateArticleDto
            {
                MaterialType = "Munskydd",
                Amount = 1,
                FullAmount = 10,
                IsOrdered = false,
                Unit = Unit.st
            };

            var resultGod = await articleService.CreateArticle(newArticleGod);

            Assert.False(resultGod.IsFailure);
            Assert.Equal(ArticleStatus.Kritisk, resultGod.Data!.Status);
        }

        [Fact]
        [Trait("Create", "ArticleService")]
        public async Task CreateArticle_ShouldReturnSlutStatus_WhenAmountIsZero()
        {
            var articleService = new ArticleService(fakeRepository, fakeLogger);

            var existingArticle = new Article
            {
                Id = "1",
                MaterialType = "Munskydd",
                Amount = 0,
                FullAmount = 10,
                IsOrdered = false,
                Unit = Unit.st
            };

            A.CallTo(() => fakeRepository.AddAsync(A<Article>.Ignored))
                .ReturnsLazily(call => Task.FromResult(call.GetArgument<Article>(0)));

            var newArticleSlut = new CreateArticleDto
            {
                MaterialType = "Munskydd",
                Amount = 0,
                FullAmount = 10,
                IsOrdered = false,
                Unit = Unit.st
            };

            var resultSlut = await articleService.CreateArticle(newArticleSlut);

            Assert.False(resultSlut.IsFailure);
            Assert.Equal(ArticleStatus.Slut, resultSlut.Data!.Status);
        }

        [Fact]
        [Trait("Create", "ArticleService")]
        public async Task CreateArticle_ShouldReturnGodStatus_WhenAmountIsLittleLessThenZero()
        {
            var articleService = new ArticleService(fakeRepository, fakeLogger);

            var newArticleNotFullt = new CreateArticleDto
            {
                MaterialType = "Artikel Not Fullt",
                Amount = 7,
                FullAmount = 10,
                IsOrdered = false,
                Unit = Unit.st
            };

            A.CallTo(() => fakeRepository.AddAsync(A<Article>.Ignored))
                .ReturnsLazily(call => Task.FromResult(call.GetArgument<Article>(0)));

            var resultMellan = await articleService.CreateArticle(newArticleNotFullt);

            Assert.False(resultMellan.IsFailure);
            Assert.Equal(ArticleStatus.God, resultMellan.Data!.Status);
        }

        [Fact]
        [Trait("Update", "ArticleService")]
        public async Task UpdateArticle_ShouldReturnUpdatedArticle_WhenValidInput()
        {
            var articleService = new ArticleService(fakeRepository, fakeLogger);
            var existingArticle = new Article
            {
                Id = "1",
                MaterialType = "Munskydd",
                Amount = 10,
                FullAmount = 10,
                IsOrdered = false,
                Unit = Unit.st
            };

            A.CallTo(() => fakeRepository.GetByIdAsync("1"))
             .Returns(Task.FromResult(existingArticle));

            A.CallTo(() => fakeRepository.UpdateAsync(A<Article>.Ignored))
                .ReturnsLazily(call => Task.FromResult(call.GetArgument<Article>(0)));

            var updateDto = new UpdateArticleDto
            {
                MaterialType = "Munskydd Updated",
                Amount = 5,
                FullAmount = 15,
                IsOrdered = true,
                Unit = Unit.st
            };

            var result = await articleService.UpdateArticle("1", updateDto);

            Assert.False(result.IsFailure);
            Assert.NotNull(result.Data);
            Assert.Equal("1", result.Data.Id);
            Assert.Equal("Munskydd Updated", result.Data.MaterialType);
            Assert.Equal(5, result.Data.Amount);
            Assert.Equal(15, result.Data.FullAmount);
            Assert.True(result.Data.IsOrdered);
        }

        [Fact]
        [Trait("Update", "ArticleService")]
        public async Task UpdateArticle_ShouldReturnError_WhenArticleNotFound()
        {
            var articleService = new ArticleService(fakeRepository, fakeLogger);

            A.CallTo(() => fakeRepository.GetByIdAsync("non-existent-id"))
             .Returns(Task.FromResult<Article?>(null));

            var updateDto = new UpdateArticleDto
            {
                MaterialType = "NonExistent",
                Amount = 5,
                FullAmount = 10,
                IsOrdered = false,
                Unit = Unit.st
            };

            var result = await articleService.UpdateArticle("non-existent-id", updateDto);

            Assert.True(result.IsFailure);
            Assert.Null(result.Data);
            Assert.Equal($"Article with Id non-existent-id not found.", result.ErrorMessage);
        }

        [Fact]
        [Trait("Update", "ArticleService")]
        public async Task UpdateArticle_ShouldReturnError_WhenAmountIsNegative()
        {
            var articleService = new ArticleService(fakeRepository, fakeLogger);
            var existingArticle = new Article
            {
                Id = "1",
                MaterialType = "Munskydd",
                Amount = 10,
                FullAmount = 10,
                IsOrdered = false,
                Unit = Unit.st
            };

            A.CallTo(() => fakeRepository.GetByIdAsync("1"))
             .Returns(Task.FromResult(existingArticle));

            var updateDto = new UpdateArticleDto
            {
                MaterialType = "Munskydd",
                Amount = -5,
                FullAmount = 10,
                IsOrdered = false,
                Unit = Unit.st
            };
            var result = await articleService.UpdateArticle("1", updateDto);

            Assert.True(result.IsFailure);
            Assert.Null(result.Data);
            Assert.Equal("Amount must be zero or greater.", result.ErrorMessage);
        }

        [Fact]
        [Trait("Update", "ArticleService")]
        public async Task UpdateArticle_ShouldReturnError_WhenFullAmountIsNegative()
        {
            var articleService = new ArticleService(fakeRepository, fakeLogger);
            var existingArticle = new Article
            {
                Id = "1",
                MaterialType = "Munskydd",
                Amount = 10,
                FullAmount = 10,
                IsOrdered = false,
                Unit = Unit.st
            };

            A.CallTo(() => fakeRepository.GetByIdAsync("1"))
             .Returns(Task.FromResult(existingArticle));

            var updateDto = new UpdateArticleDto
            {
                MaterialType = "Munskydd",
                Amount = 5,
                FullAmount = -10,
                IsOrdered = false,
                Unit = Unit.st
            };

            var result = await articleService.UpdateArticle("1", updateDto);

            Assert.True(result.IsFailure);
            Assert.Null(result.Data);
            Assert.Equal("FullAmount must be zero or greater.", result.ErrorMessage);
        }

        [Fact]
        [Trait("Update", "ArticleService")]
        public async Task UpdateArticle_ShouldReturnError_WhenFullAmountLessThanAmount()
        {
            var articleService = new ArticleService(fakeRepository, fakeLogger);
            var existingArticle = new Article
            {
                Id = "1",
                MaterialType = "Munskydd",
                Amount = 10,
                FullAmount = 10,
                IsOrdered = false,
                Unit = Unit.st
            };

            A.CallTo(() => fakeRepository.GetByIdAsync("1"))
             .Returns(Task.FromResult(existingArticle));

            var updateDto = new UpdateArticleDto
            {
                MaterialType = "Munskydd",
                Amount = 15,
                FullAmount = 10,
                IsOrdered = false,
                Unit = Unit.st
            };

            var result = await articleService.UpdateArticle("1", updateDto);

            Assert.True(result.IsFailure);
            Assert.Null(result.Data);
            Assert.Equal("Amount cannot exceed FullAmount.", result.ErrorMessage);
        }

        [Fact]
        [Trait("Delete", "ArticleService")]
        public async Task DeleteArticle_ShouldReturnTrue_WhenArticleDeleted()
        {
            var articleService = new ArticleService(fakeRepository, fakeLogger);
            var existingArticle = new Article
            {
                Id = "1",
                MaterialType = "Munskydd",
                Amount = 10,
                FullAmount = 10,
                IsOrdered = false,
                Unit = Unit.st
            };

            A.CallTo(() => fakeRepository.GetByIdAsync("1"))
             .Returns(Task.FromResult(existingArticle));

            A.CallTo(() => fakeRepository.DeleteAsync("1"))
             .Returns(Task.FromResult(true));

            var result = await articleService.DeleteArticle("1");

            Assert.False(result.IsFailure);
            Assert.Equal($"Article {existingArticle.MaterialType} was deleted successfully.", result.Data);
        }

        [Fact]
        [Trait("Delete", "ArticleService")]
        public async Task DeleteArticle_ShouldReturnError_WhenArticleNotFound()
        {
            var articleService = new ArticleService(fakeRepository, fakeLogger);

            A.CallTo(() => fakeRepository.GetByIdAsync("non-existent-id"))
             .Returns(Task.FromResult<Article?>(null));

            var result = await articleService.DeleteArticle("non-existent-id");

            Assert.True(result.IsFailure);
            Assert.Null(result.Data);
            Assert.Equal($"Article with Id non-existent-id not found.", result.ErrorMessage);
        }

        [Fact]
        [Trait("Delete", "ArticleService")]
        public async Task DeleteArticle_ShouldReturnError_WhenDeletionFails()
        {
            var articleService = new ArticleService(fakeRepository, fakeLogger);
            var existingArticle = new Article
            {
                Id = "1",
                MaterialType = "Munskydd",
                Amount = 10,
                FullAmount = 10,
                IsOrdered = false,
                Unit = Unit.st
            };

            A.CallTo(() => fakeRepository.GetByIdAsync("1"))
             .Returns(Task.FromResult(existingArticle));

            A.CallTo(() => fakeRepository.DeleteAsync("1"))
             .Returns(Task.FromResult(false));

            var result = await articleService.DeleteArticle("1");

            Assert.True(result.IsFailure);
            Assert.Null(result.Data);
            Assert.Equal($"Failed to delete Article {existingArticle.MaterialType}.", result.ErrorMessage);
        }

        [Fact]
        [Trait("GetById", "ArticleService")]
        public async Task GetByIdArticles_ShouldReturnArticle()
        {
            var articleService = new ArticleService(fakeRepository, fakeLogger);
            var existingArticle = new Article
            {
                Id = "1",
                MaterialType = "Munskydd",
                Amount = 10,
                FullAmount = 10,
                IsOrdered = false,
                Unit = Unit.st
            };

            A.CallTo(() => fakeRepository.GetByIdAsync("1"))
             .Returns(Task.FromResult(existingArticle));

            var result = await articleService.GetById("1");

            Assert.False(result.IsFailure);
            Assert.NotNull(result.Data);
            Assert.Equal("1", result.Data!.Id);
            Assert.Equal("Munskydd", result.Data.MaterialType);
        }

        [Fact]
        [Trait("GetById", "ArticleService")]
        public async Task GetByIdArticles_ShouldReturnError_WhenArticleNotFound()
        {
            var articleService = new ArticleService(fakeRepository, fakeLogger);

            A.CallTo(() => fakeRepository.GetByIdAsync("non-existent-id"))
             .Returns(Task.FromResult<Article?>(null));

            var result = await articleService.GetById("non-existent-id");

            Assert.True(result.IsFailure);
            Assert.Null(result.Data);
            Assert.Equal("Article not found.", result.ErrorMessage);
        }

        [Fact]
        [Trait("GetAll", "ArticleService")]
        public async Task GetAllArticles_ShouldReturnListOfArticles()
        {
            var articleService = new ArticleService(fakeRepository, fakeLogger);
            var articles = new List<Article>
            {
                new ()
                {
                    Id = "2",
                    MaterialType = "Handskar",
                    Amount = 20,
                    FullAmount = 20,
                    IsOrdered = false,
                    Unit = Unit.st,
                    Status = ArticleStatus.Fullt
                },
                new ()
                {
                    Id = "1",
                    MaterialType = "Munskydd",
                    Amount = 3,
                    FullAmount = 10,
                    IsOrdered = false,
                    Unit = Unit.st,
                    Status = ArticleStatus.Kritisk
                }
            };

            A.CallTo(() => fakeRepository.GetAllAsync())
             .Returns(Task.FromResult(articles.AsEnumerable().ToList()));

            var result = await articleService.GetAll();
            
            Assert.False(result.IsFailure);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data!.Count);
        }
    }
}
