using System.ComponentModel.DataAnnotations;

namespace RecipeApi.Models
{
    public class RecipeIngredient
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public int IngredientId { get; set; }
        
        [Required]
        public decimal Quantity { get; set; }
        
        [MaxLength(20)]
        public string? Unit { get; set; }
        
        [MaxLength(200)]
        public string? Notes { get; set; }
        
        // Navigation properties
        public virtual Recipe Recipe { get; set; } = null!;
        public virtual Ingredient Ingredient { get; set; } = null!;
    }
}