using Domain.Enums;

namespace Domain.Models
{
    public class Article
    {
        public string Id { get; set; } = string.Empty;
        public string MaterialType { get; set; } = string.Empty;
        public int Amount { get; set; }
        public int FullAmount { get; set; }
        public bool IsOrdered { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public Unit Unit { get; set; } = Unit.st;
        public ArticleStatus Status { get; set; } 
    }
}
