using System.ComponentModel.DataAnnotations;

namespace RecipeApi.DTOs
{
    public class CreateRecipeRatingDto
    {
        [Required]
        public int RecipeId { get; set; }
        
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }
        
        [MaxLength(1000)]
        public string? Review { get; set; }
    }
} 