using Application.Dtos;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticleController(ArticleService articleService) : Controller
    {
        private readonly ArticleService _articleService = articleService;

        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateArticleDto articleDto)
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

        [HttpPut("{id:string}")]
        public async Task<IActionResult> Update(string id, UpdateArticleDto articleDto)
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

        [HttpDelete("{id:string}")]
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

        [HttpGet("GetById")]
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
    }
}
