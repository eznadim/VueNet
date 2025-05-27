using System.ComponentModel.DataAnnotations;
using RecipeApi.Models;

namespace RecipeApi.DTOs
{
    public class CreateRecipe
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        [Required]
        public string Instructions { get; set; } = string.Empty;
        
        public int? PrepTimeMinutes { get; set; }
        public int? CookTimeMinutes { get; set; }
        public int? Servings { get; set; }
        public Difficulty Difficulty { get; set; } = Difficulty.Easy;
        public string? ImageUrl { get; set; }
        public int? CategoryId { get; set; }
        
        public List<CreateRecipeIngredient> Ingredients { get; set; } = new List<CreateRecipeIngredient>();
        public List<int> TagIds { get; set; } = new List<int>();
    }
}