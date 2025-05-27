using System.ComponentModel.DataAnnotations;

namespace RecipeApi.Models
{
    public class RecipeRating
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public int UserId { get; set; }
        
        [Range(1, 5)]
        public int Rating { get; set; }
        
        public string? Review { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual Recipe Recipe { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}