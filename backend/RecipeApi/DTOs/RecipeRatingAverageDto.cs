namespace RecipeApi.DTOs
{
    public class RecipeRatingAverageDto
    {
        public int RecipeId { get; set; }
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
    }
} 