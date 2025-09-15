using Application.Dtos;
using Application.Dtos.ReponsObjects;
using Application.Interfaces;
using Domain.Models;
using Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class ArticleService(IGenericRepository<Article> articleRepository, ILogger<ArticleService> logger)
    {
        private static ArticleStatus CalculateStatus(int amount, int fullAmount)
        {
            var safeMax = fullAmount > 0 ? fullAmount : 250; // fallback to 250 to avoid divide by zero
            
            if (amount >= safeMax) return ArticleStatus.Fullt;
            if (amount <= 0) return ArticleStatus.Slut;
            
            var ratio = (double)amount / safeMax;

            if (ratio >= 0.7d) return ArticleStatus.God;
            if (ratio >= 0.4d) return ArticleStatus.Mellan;
           
            return ArticleStatus.Kritisk;
        }

        private static string NormalizeMaterialType(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            var trimmed = input.Trim();

            if (trimmed.Length == 1) return trimmed.ToUpperInvariant();

            return char.ToUpperInvariant(trimmed[0]) + trimmed[1..].ToLowerInvariant();
        }

        public async Task<OperationResult<Article>> CreateArticle(CreateArticleDto articleDto)
        {
            try
            {
                if (articleDto is not { MaterialType.Length: > 0, Amount: > -1, FullAmount: > -1 })
                {
                    return OperationResult<Article>.FailureResult(
                        articleDto is null ? "Input DTO cannot be null."
                        : string.IsNullOrWhiteSpace(articleDto.MaterialType) ? "MaterialType is required."
                        : articleDto.Amount < 0 ? "Amount must be zero or greater."
                        : articleDto.FullAmount < 0 ? "FullAmount must be zero or greater."
                        : "Invalid input.");
                }

                if(articleDto.Amount > articleDto.FullAmount)
                {
                    logger.LogError("Attempted to create Article with MaterialType {MaterialType} where Amount {Amount} exceeds FullAmount {FullAmount}.", articleDto.MaterialType, articleDto.Amount, articleDto.FullAmount);
                    return OperationResult<Article>.FailureResult("Amount cannot exceed FullAmount.");
                }

                var formattedMaterialType = NormalizeMaterialType(articleDto.MaterialType);

                var existingArticle = await articleRepository.FindAsync(a => a.MaterialType.Equals(formattedMaterialType));

                if (existingArticle.Count is not 0)
                {
                    logger.LogError("Attempted to create duplicate Article with MaterialType {MaterialType}.", articleDto.MaterialType);
                    return OperationResult<Article>.FailureResult($"Article with MaterialType {articleDto.MaterialType} already exists.");
                }


                var article = new Article
                {
                    Id = Guid.NewGuid().ToString(),
                    MaterialType = formattedMaterialType,
                    Amount = articleDto.Amount,
                    FullAmount = articleDto.FullAmount,
                    IsOrdered = articleDto.IsOrdered,
                    Unit = articleDto.Unit,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Status = CalculateStatus(articleDto.Amount, articleDto.FullAmount)
                };


                var savedArticle = await articleRepository.AddAsync(article);

                if (savedArticle is null)
                {
                    logger.LogError("Failed to create Article with MaterialType {MaterialType}.", article.MaterialType);
                    return OperationResult<Article>.FailureResult($"Failed to add the Article: {article.MaterialType}.");
                }

                return OperationResult<Article>.SuccessResult(savedArticle);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while creating an article.");
                return OperationResult<Article>.FailureResult("An unexpected error creating Article.");
            }
        }

        public async Task<OperationResult<Article>> UpdateArticle(string articleId, UpdateArticleDto articleDto)
        {
            try
            {
                if (articleDto is not { MaterialType.Length: > 0, Amount: > -1, FullAmount: > -1 })
                {
                    return OperationResult<Article>.FailureResult(
                        articleDto is null ? "Input DTO cannot be null."
                        : string.IsNullOrWhiteSpace(articleId) ? "Valid Id is required."
                        : string.IsNullOrWhiteSpace(articleDto.MaterialType) ? "MaterialType is required."
                        : articleDto.Amount < 0 ? "Amount must be zero or greater."
                        : articleDto.FullAmount < 0 ? "FullAmount must be zero or greater."
                        : "Amount can not be negative.");
                }

                var existingArticle = await articleRepository.GetByIdAsync(articleId);

                if (existingArticle is null)
                {
                    logger.LogError("Attempted to update Article with Id {ArticleId}, but it was not found.", articleId);   
                    return OperationResult<Article>.FailureResult($"Article with Id {articleId} not found.");
                }

                if(articleDto.Amount > existingArticle.FullAmount)
                {
                    logger.LogError("Attempted to update Article with Id {ArticleId} where Amount {Amount} exceeds FullAmount {FullAmount}.", articleId, articleDto.Amount, existingArticle.FullAmount);
                    return OperationResult<Article>.FailureResult("Amount cannot exceed FullAmount.");
                }

                existingArticle.MaterialType = NormalizeMaterialType(articleDto.MaterialType);
                existingArticle.Amount = articleDto.Amount;
                existingArticle.FullAmount = articleDto.FullAmount;
                existingArticle.IsOrdered = articleDto.IsOrdered;
                existingArticle.Unit = articleDto.Unit;
                existingArticle.Status = CalculateStatus(existingArticle.Amount, existingArticle.FullAmount);
                existingArticle.UpdatedAt = DateTime.UtcNow;

                var updatedArticle = await articleRepository.UpdateAsync(existingArticle);

                if (updatedArticle is null)
                {
                    logger.LogError("Failed to update Article with Id {ArticleId}.", existingArticle.Id);
                    return OperationResult<Article>.FailureResult($"Failed to update the Article with Id: {existingArticle.Id}.");
                }

                return OperationResult<Article>.SuccessResult(updatedArticle);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while updating article {ArticleId}.", articleId);
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
                    logger.LogError("Attempted to delete Article with Id {ArticleId}, but it was not found.", articleId);
                    return OperationResult<string>.FailureResult($"Article with Id {articleId} not found.");
                }

                var isDeleted = await articleRepository.DeleteAsync(existingArticle.Id);

                if (isDeleted)
                {
                    logger.LogInformation("Article with Id {ArticleId} deleted successfully.", articleId);
                    return OperationResult<string>.SuccessResult($"Article {existingArticle.MaterialType} was deleted successfully.");
                }

                return OperationResult<string>.FailureResult($"Failed to delete Article {existingArticle.MaterialType}.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while deleting article {ArticleId}.", articleId);
                return OperationResult<string>.FailureResult("An unexpected error deleting Article.");
            }
        }

        public async Task<OperationResult<Article>> GetById(string articleId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(articleId))
                {
                    logger.LogError("GetById called with null or empty articleId.");
                    return OperationResult<Article>.FailureResult("Valid Article ID is required.");
                }

                var article = await articleRepository.GetByIdAsync(articleId);

                if (article is null)
                {
                    logger.LogError("Article with Id {ArticleId} not found.", articleId);
                    return OperationResult<Article>.FailureResult($"Article not found.");
                }

                return OperationResult<Article>.SuccessResult(article);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while retrieving article by Id {ArticleId}.", articleId);
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

                if (articleList is null || articleList.Count is 0)
                {
                    logger.LogInformation("No articles found in the repository.");
                    return OperationResult<List<Article>>.FailureResult("No articles found.");
                }

                return OperationResult<List<Article>>.SuccessResult(articleList);
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Error occurred while retrieving all articles.");
                return OperationResult<List<Article>>.FailureResult("Something went wrong-");
            }
        }

        public async Task<OperationResult<OrderReponse>> OrderArticle(string id, int amount)
        {
            try
            {
                var foundArticle = await articleRepository.GetByIdAsync(id);

                if(foundArticle is null)
                {
                    logger.LogError("Attempted to order Article with Id {ArticleId}, but it was not found.", id);
                    return OperationResult<OrderReponse>.FailureResult($"Article with the given Id was not found.");
                }

                var spaceLeft = foundArticle.FullAmount - foundArticle.Amount;

                if (amount > spaceLeft)
                {
                    logger.LogError("Attempted to order {OrderAmount} {Unit} for {foundArticle.MaterialType}, but only {SpaceLeft} {Unit} can be ordered to reach full capacity.", amount, foundArticle.Unit, foundArticle.MaterialType, spaceLeft, foundArticle.Unit);
                    return OperationResult<OrderReponse>.FailureResult($"Cannot order {amount} {foundArticle.Unit}. Only {spaceLeft} {foundArticle.Unit} can be ordered to reach full capacity.");
                }

                foundArticle.Amount += amount;
                foundArticle.UpdatedAt = DateTime.UtcNow;
                foundArticle.IsOrdered = true;
                foundArticle.Status = CalculateStatus(foundArticle.Amount, foundArticle.FullAmount);

                OrderReponse response = new()
                {
                    OrderTimeStamp = DateTime.UtcNow,
                    Article = foundArticle,
                };

                var updatedArticle = await articleRepository.UpdateAsync(foundArticle);

                if (updatedArticle is null)
                {
                    return OperationResult<OrderReponse>.FailureResult($"Failed to order the Article with Id: {foundArticle.Id}.");
                }

                return OperationResult<OrderReponse>.SuccessResult(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while ordering article {ArticleId}.", id);
                return OperationResult<OrderReponse>.FailureResult("An unexpected error ordering Article.");
            }
        }
    }
}
