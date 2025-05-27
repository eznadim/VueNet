using System.ComponentModel.DataAnnotations;

namespace RecipeApi.Models
{
    public class Recipe
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public string Instructions { get; set; } = string.Empty;

        public int? PrepTimeMinutes { get; set; }
        public int? CookTimeMinutes { get; set; }
        public int? Servings { get; set; }

        public Difficulty Difficulty { get; set; } = Difficulty.Easy;

        public string? ImageUrl { get; set; }

        public int UserId { get; set; }
        public int? CategoryId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User User { get; set; } = null!;
        public Category? Category { get; set; }
        public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
        public ICollection<RecipeTag> RecipeTags { get; set; } = new List<RecipeTag>();
        public ICollection<RecipeRating> RecipeRatings { get; set; } = new List<RecipeRating>();
    }
}