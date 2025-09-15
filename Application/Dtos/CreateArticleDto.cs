using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos
{
    public class CreateArticleDto
    {
        [Required]
        [StringLength(200)]
        public string MaterialType { get; set; } = string.Empty;
        [Range(0,int.MaxValue)]
        public int Amount { get; set; }
        [Range(0,int.MaxValue)]
        public int FullAmount { get; set; }
        public bool IsOrdered { get; set; } = false;
        public Unit Unit { get; set; } = Unit.st;
    }
}
