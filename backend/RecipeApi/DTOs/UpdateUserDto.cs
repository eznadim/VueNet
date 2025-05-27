using System.ComponentModel.DataAnnotations;

namespace RecipeApi.DTOs
{
    public class UpdateUserDto
    {
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;
    }
} 