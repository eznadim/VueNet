using RecipeApi.DTOs;

namespace RecipeApi.Services
{
    public interface IRecipeService
    {
        Task<IEnumerable<RecipeDto>> GetAllRecipesAsync(int? categoryId = null, string? searchTerm = null, int page = 1, int pageSize = 10);
        Task<RecipeDto?> GetRecipeByIdAsync(int id);
        Task<RecipeDto> CreateRecipeAsync(CreateRecipe request, int userId);
        Task<RecipeDto?> UpdateRecipeAsync(int id, UpdateRecipe request, int userId);
        Task<bool> DeleteRecipeAsync(int id, int userId);
        Task<IEnumerable<RecipeDto>> GetUserRecipesAsync(int userId);
    }
}