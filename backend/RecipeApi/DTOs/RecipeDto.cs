using RecipeApi.Models;

namespace RecipeApi.DTOs
{
    public class RecipeDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Instructions { get; set; } = string.Empty;
        public int? PrepTimeMinutes { get; set; }
        public int? CookTimeMinutes { get; set; }
        public int? Servings { get; set; }
        public Difficulty Difficulty { get; set; }
        public string? ImageUrl { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        public List<RecipeIngredientDto> Ingredients { get; set; } = new List<RecipeIngredientDto>();
        public List<TagDto> Tags { get; set; } = new List<TagDto>();
        public double? AverageRating { get; set; }
        public int RatingCount { get; set; }
    }
}