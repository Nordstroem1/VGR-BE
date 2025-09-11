using Application.Dtos;
using Domain.Models;
using Application.Interfaces;

namespace Application.Services
{
    public class ArticleService(IGenericRepository<Article> articleRepository)
    {
        public async Task<OperationResult<Article>> CreateArticle(CreateArticleDto articleDto)
        {
            try
            {
                if (articleDto is null)
                {
                    return OperationResult<Article>.FailureResult("Input DTO cannot be null.");
                }

                if (string.IsNullOrWhiteSpace(articleDto.MaterialType))
                {
                    return OperationResult<Article>.FailureResult("MaterialType is required.");
                }

                if (articleDto.Amount <= 0)
                {
                    return OperationResult<Article>.FailureResult("Amount must be greater than zero.");
                }

                var article = new Article
                {
                    Id = Guid.NewGuid(),
                    MaterialType = articleDto.MaterialType.Trim(),
                    Amount = articleDto.Amount,
                    IsOrdered = articleDto.IsOrdered,
                    Unit = articleDto.Unit,
                    Status = articleDto.Status,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                return await articleRepository.AddAsync(article);
            }
            catch (Exception ex)
            {
                return OperationResult<Article>.FailureResult($"Unexpected error creating Article: {ex.Message}");
            }
        }
    }
}
