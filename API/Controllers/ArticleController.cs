using Application.Dtos;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticleController([FromBody] ArticleService articleService) : Controller
    {
        private readonly ArticleService _articleService = articleService;

        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateArticleDto articleDto)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var creationResult = await _articleService.CreateArticle(articleDto);

                if (creationResult.IsFailure || creationResult.Data is null)
                {
                    return BadRequest(creationResult.ErrorMessage);
                }

                return Ok(creationResult.Data);
            }
            catch
            {
                return StatusCode(500, "An unexpected error occurred while retrieving the article.");

            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, UpdateArticleDto articleDto)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updateResult = await _articleService.UpdateArticle(id, articleDto);

                if (updateResult.IsFailure || updateResult.Data is null)
                {
                    return BadRequest(updateResult.ErrorMessage);
                }

                return Ok(updateResult.Data);
            }
            catch
            {
                return StatusCode(500, "An unexpected error occurred while retrieving the article.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest("Article ID is required.");
                }

                var deletionResult = await _articleService.DeleteArticle(id);

                if (deletionResult.IsFailure)
                {
                    return BadRequest(deletionResult.ErrorMessage);
                }

                return NoContent();
            }
            catch
            {
                return StatusCode(500, "An unexpected error occurred while deleting the article.");
            }
        }

        [HttpGet("GetById/{id}")] //behåller för nu.
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest("Article id is required.");
                }

                var retrievalResult = await _articleService.GetById(id);

                if (retrievalResult.IsFailure || retrievalResult.Data is null)
                {
                    return NotFound(retrievalResult.ErrorMessage);
                }

                return Ok(retrievalResult.Data);
            }
            catch
            {
                return StatusCode(500, "An unexpected error occurred while retrieving the article.");
            }
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var retrievalResult = await _articleService.GetAll();

                if (retrievalResult.IsFailure || retrievalResult.Data is null)
                {
                    return NotFound(retrievalResult.ErrorMessage);
                }

                return Ok(retrievalResult.Data);
            }
            catch
            {
                return StatusCode(500, "An unexpected error occurred while retrieving articles.");
            }
        }

        [HttpPost("OrderArticle/{id}")]
        public async Task<IActionResult> OrderArticle(string id, int Amount)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest("Article ID is required.");
                }

                var orderResult = await _articleService.OrderArticle(id, Amount);
                
                if (orderResult.IsFailure || orderResult.Data is null)
                {
                    return BadRequest(orderResult.ErrorMessage);
                }

                return Ok(orderResult.Data);
            }
            catch
            {
                return StatusCode(500, "An unexpected error occurred while ordering the article.");
            }
        }
    }
}
