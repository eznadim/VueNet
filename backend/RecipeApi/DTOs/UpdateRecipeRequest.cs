using System.ComponentModel.DataAnnotations;

namespace RecipeApi.DTOs
{
    public class UpdateRecipeRequest
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [Required]
        public string Instructions { get; set; } = string.Empty;
        
        [Range(1, int.MaxValue)]
        public int PrepTimeMinutes { get; set; }
        
        [Range(1, int.MaxValue)]
        public int CookTimeMinutes { get; set; }
        
        [Range(1, int.MaxValue)]
        public int Servings { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Difficulty { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? ImageUrl { get; set; }
        
        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }
        
        public List<CreateRecipeIngredient> Ingredients { get; set; } = new List<CreateRecipeIngredient>();
        
        public List<string> Tags { get; set; } = new List<string>();
    }
} 