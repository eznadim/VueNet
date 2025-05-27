namespace RecipeApi.DTOs
{
    public class IngredientDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Unit { get; set; }
    }
}