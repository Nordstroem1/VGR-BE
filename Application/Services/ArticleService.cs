using Application.Dtos;
using Domain.Models;
using Application.Interfaces;
using Domain.Enums;

namespace Application.Services
{
    public class ArticleService(IGenericRepository<Article> articleRepository)
    {
        public async Task<OperationResult<Article>> CreateArticle(CreateArticleDto articleDto)
        {
            try
            {
                if (articleDto is not { MaterialType.Length: > 0, Amount: > -1 })
                {
                    return OperationResult<Article>.FailureResult(
                        articleDto is null ? "Input DTO cannot be null."
                        : string.IsNullOrWhiteSpace(articleDto.MaterialType) ? "MaterialType is required."
                        : "Amount must be greater than zero.");
                }

                var article = new Article
                {
                    Id = Guid.NewGuid().ToString(),
                    MaterialType = articleDto.MaterialType.Trim(),
                    Amount = articleDto.Amount,
                    IsOrdered = articleDto.IsOrdered,
                    Unit = articleDto.Unit,
                    Status = articleDto.Status,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };


                var saved = await articleRepository.AddAsync(article);

                if (saved is null)
                {
                    return OperationResult<Article>.FailureResult($"Failed to add the Article: {article.MaterialType}.");
                }

                return OperationResult<Article>.SuccessResult(saved);
            }
            catch
            {
                return OperationResult<Article>.FailureResult("An unexpected error creating Article.");
            }
        }

        public async Task<OperationResult<Article>> UpdateArticle(string articleId ,UpdateArticleDto articleDto)
        {
            try
            {
                if (articleDto is not { MaterialType.Length: > 0, Amount: > -1 })
                {
                    return OperationResult<Article>.FailureResult(
                        articleDto is null ? "Input DTO cannot be null."
                        : string.IsNullOrWhiteSpace(articleId) ? "Valid Id is required."
                        : string.IsNullOrWhiteSpace(articleDto.MaterialType) ? "MaterialType is required."
                        : "Amount can not be negative.");
                }

                var existingArticle = await articleRepository.GetByIdAsync(articleId);
                
                if (existingArticle is null)
                {
                    return OperationResult<Article>.FailureResult($"Article with Id {articleId} not found.");
                }

                existingArticle.MaterialType = articleDto.MaterialType.Trim();
                existingArticle.Amount = articleDto.Amount;
                existingArticle.IsOrdered = articleDto.IsOrdered;
                existingArticle.Unit = articleDto.Unit;
                existingArticle.Status = articleDto.Status;
                existingArticle.UpdatedAt = DateTime.UtcNow;

                var updatedOperation = await articleRepository.UpdateAsync(existingArticle);

                if (updatedOperation is null)
                {
                    return OperationResult<Article>.FailureResult($"Failed to update the Article with Id: {existingArticle.Id}.");
                }

                return OperationResult<Article>.SuccessResult(updatedOperation);
            }
            catch
            {
                return OperationResult<Article>.FailureResult("An unexpected error updating Article.");
            }
        }
    }
}
