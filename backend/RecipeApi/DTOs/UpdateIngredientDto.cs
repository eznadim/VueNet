using System.ComponentModel.DataAnnotations;

namespace RecipeApi.DTOs
{
    public class UpdateIngredientDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string? Unit { get; set; }
    }
} 