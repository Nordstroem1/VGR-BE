using Application.Dtos;
using Application.Interfaces;
using Domain.Models;

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

                var existingArticle = await articleRepository.FindAsync(a => a.MaterialType.Equals(articleDto.MaterialType.Trim(), StringComparison.CurrentCultureIgnoreCase));

                if (existingArticle.Count is not 0)
                {
                    return OperationResult<Article>.FailureResult($"Article with MaterialType {articleDto.MaterialType} already exists.");
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


                var savedArticle = await articleRepository.AddAsync(article);

                if (savedArticle is null)
                {
                    return OperationResult<Article>.FailureResult($"Failed to add the Article: {article.MaterialType}.");
                }

                return OperationResult<Article>.SuccessResult(savedArticle);
            }
            catch
            {
                return OperationResult<Article>.FailureResult("An unexpected error creating Article.");
            }
        }

        public async Task<OperationResult<Article>> UpdateArticle(string articleId, UpdateArticleDto articleDto)
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

                var updatedArticle = await articleRepository.UpdateAsync(existingArticle);

                if (updatedArticle is null)
                {
                    return OperationResult<Article>.FailureResult($"Failed to update the Article with Id: {existingArticle.Id}.");
                }

                return OperationResult<Article>.SuccessResult(updatedArticle);
            }
            catch
            {
                return OperationResult<Article>.FailureResult("An unexpected error updating Article.");
            }
        }

        public async Task<OperationResult<string>> DeleteArticle(string articleId)
        {
            try
            {
                var existingArticle = await articleRepository.GetByIdAsync(articleId);

                if (existingArticle is null)
                {
                    return OperationResult<string>.FailureResult($"Article with Id {articleId} not found.");
                }

                var isDeleted = await articleRepository.DeleteAsync(existingArticle.Id);

                if (isDeleted)
                {
                    return OperationResult<string>.SuccessResult($"Article {existingArticle.MaterialType} was deleted successfully.");
                }

                return OperationResult<string>.FailureResult($"Failed to delete the Article with Id: {articleId}.");
            }
            catch
            {
                return OperationResult<string>.FailureResult("An unexpected error deleting Article.");
            }
        }

        public async Task<OperationResult<Article>> GetById(string articleId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(articleId))
                {
                    return OperationResult<Article>.FailureResult("Valid Article ID is required.");
                }

                var article = await articleRepository.GetByIdAsync(articleId);

                if (article is null)
                {
                    return OperationResult<Article>.FailureResult($"Article not found.");
                }

                return OperationResult<Article>.SuccessResult(article);
            }
            catch
            {
                return OperationResult<Article>.FailureResult("An unexpected error retrieving Article by Id.");
            }
        }

        public async Task<OperationResult<List<Article>>> GetAll()
        {
            try
            {
                var articleList = (await articleRepository.GetAllAsync())
                                                          .OrderByDescending(a => a.Status)
                                                          .ToList();

                if(articleList is null || articleList.Count is 0)
                {
                    return OperationResult<List<Article>>.FailureResult("No articles found.");
                }

                return OperationResult<List<Article>>.SuccessResult(articleList);
            }
            catch
            {
                return OperationResult<List<Article>>.FailureResult("Something went wrong-");
            }
        }
    }
}
