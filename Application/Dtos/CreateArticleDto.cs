using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos
{
    public class CreateArticleDto
    {
        [Required]
        [StringLength(200)]
        public string MaterialType { get; set; } = string.Empty;
        [Range(1,int.MaxValue)]
        public int Amount { get; set; }
        public bool IsOrdered { get; set; } = false;
        public Unit Unit { get; set; } = Unit.st;
        public ArticleStatus Status = ArticleStatus.Fullt;
    }
}
