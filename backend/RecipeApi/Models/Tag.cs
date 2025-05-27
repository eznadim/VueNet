using System.ComponentModel.DataAnnotations;

namespace RecipeApi.Models
{
    public class Tag
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(7)]
        public string Color { get; set; } = "#007bff";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual ICollection<RecipeTag> RecipeTags { get; set; } = new List<RecipeTag>();
    }
}