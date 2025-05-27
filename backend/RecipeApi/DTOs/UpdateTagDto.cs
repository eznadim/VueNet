using System.ComponentModel.DataAnnotations;

namespace RecipeApi.DTOs
{
    public class UpdateTagDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(7)]
        public string? Color { get; set; }
    }
} 