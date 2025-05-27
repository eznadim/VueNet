namespace RecipeApi.Models
{
    public class RecipeTag
    {
        public int RecipeId { get; set; }
        public int TagId { get; set; }
        
        // Navigation properties
        public virtual Recipe Recipe { get; set; } = null!;
        public virtual Tag Tag { get; set; } = null!;
    }
}