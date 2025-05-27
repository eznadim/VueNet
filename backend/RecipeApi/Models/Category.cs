using System.ComponentModel.DataAnnotations;

namespace RecipeApi.Models
{
    public class Category
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
    }
}