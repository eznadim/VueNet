using System.ComponentModel.DataAnnotations;

namespace RecipeApi.DTOs
{
    public class CreateCategoryDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Description { get; set; }
    }
} 