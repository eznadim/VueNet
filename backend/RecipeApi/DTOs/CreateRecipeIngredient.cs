using System.ComponentModel.DataAnnotations;

namespace RecipeApi.DTOs
{
    public class CreateRecipeIngredient
    {
        public int? IngredientId { get; set; }
        public string? IngredientName { get; set; }
        
        [Required]
        public decimal Quantity { get; set; }
        
        public string? Unit { get; set; }
        public string? Notes { get; set; }
    }
}