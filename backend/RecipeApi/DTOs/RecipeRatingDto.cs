namespace RecipeApi.DTOs
{
    public class RecipeRatingDto
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Review { get; set; }
        public DateTime CreatedAt { get; set; }
    }
} 