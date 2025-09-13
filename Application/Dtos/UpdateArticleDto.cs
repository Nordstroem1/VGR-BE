using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos
{
    public class UpdateArticleDto
    {
        [Required]
        [StringLength(200)]
        public string MaterialType { get; set; } = string.Empty;
        [Range(1, int.MaxValue)]
        public int Amount { get; set; }
        public bool IsOrdered { get; set; } = false;
        [Required]
        public Unit Unit { get; set; } 
        [Required]
        public ArticleStatus Status { get; set; } 
    }
}
