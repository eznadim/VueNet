namespace RecipeApi.DTOs
{
    public class RecipeIngredientDto
    {
        public int Id { get; set; }
        public int IngredientId { get; set; }
        public string IngredientName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string? Unit { get; set; }
        public string? Notes { get; set; }
    }
}