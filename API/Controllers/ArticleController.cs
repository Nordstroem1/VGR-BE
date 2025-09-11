using Microsoft.AspNetCore.Mvc;
using Application.Dtos;
using Application.Services;
using Domain.Models;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticleController : Controller
    {
        private readonly ArticleService _articleService;
        public ArticleController(ArticleService articleService)
        {
            _articleService = articleService;
        }

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
    }
}
