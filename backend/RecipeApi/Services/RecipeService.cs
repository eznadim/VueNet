using Microsoft.EntityFrameworkCore;
using AutoMapper;
using RecipeApi.Data;
using RecipeApi.DTOs;
using RecipeApi.Models;

namespace RecipeApi.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly RecipeDbContext _context;
        private readonly IMapper _mapper;

        public RecipeService(RecipeDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RecipeDto>> GetAllRecipesAsync(int? categoryId = null, string? searchTerm = null, int page = 1, int pageSize = 10)
        {
            var query = _context.Recipes
                .Include(r => r.User)
                .Include(r => r.Category)
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .Include(r => r.RecipeTags)
                    .ThenInclude(rt => rt.Tag)
                .Include(r => r.RecipeRatings)
                .AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(r => r.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(r => r.Title.Contains(searchTerm) || 
                                       r.Description!.Contains(searchTerm) ||
                                       r.RecipeIngredients.Any(ri => ri.Ingredient.Name.Contains(searchTerm)));
            }

            var recipes = await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return recipes.Select(MapToRecipeDto);
        }

        public async Task<RecipeDto?> GetRecipeByIdAsync(int id)
        {
            var recipe = await _context.Recipes
                .Include(r => r.User)
                .Include(r => r.Category)
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .Include(r => r.RecipeTags)
                    .ThenInclude(rt => rt.Tag)
                .Include(r => r.RecipeRatings)
                .FirstOrDefaultAsync(r => r.Id == id);

            return recipe == null ? null : MapToRecipeDto(recipe);
        }

        public async Task<RecipeDto> CreateRecipeAsync(CreateRecipe request, int userId)
        {
            var recipe = new Recipe
            {
                Title = request.Title,
                Description = request.Description,
                Instructions = request.Instructions,
                PrepTimeMinutes = request.PrepTimeMinutes,
                CookTimeMinutes = request.CookTimeMinutes,
                Servings = request.Servings,
                Difficulty = request.Difficulty,
                ImageUrl = request.ImageUrl,
                UserId = userId,
                CategoryId = request.CategoryId
            };

            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();

            // Add ingredients
            foreach (var ingredientRequest in request.Ingredients)
            {
                var ingredient = await GetOrCreateIngredientAsync(ingredientRequest);
                
                var recipeIngredient = new RecipeIngredient
                {
                    RecipeId = recipe.Id,
                    IngredientId = ingredient.Id,
                    Quantity = ingredientRequest.Quantity,
                    Unit = ingredientRequest.Unit,
                    Notes = ingredientRequest.Notes
                };

                _context.RecipeIngredients.Add(recipeIngredient);
            }

            // Add tags
            foreach (var tagId in request.TagIds)
            {
                var recipeTag = new RecipeTag
                {
                    RecipeId = recipe.Id,
                    TagId = tagId
                };
                _context.RecipeTags.Add(recipeTag);
            }

            await _context.SaveChangesAsync();

            return await GetRecipeByIdAsync(recipe.Id) ?? throw new InvalidOperationException("Failed to retrieve created recipe");
        }

        public async Task<RecipeDto?> UpdateRecipeAsync(int id, UpdateRecipe request, int userId)
        {
            var recipe = await _context.Recipes
                .Include(r => r.RecipeIngredients)
                .Include(r => r.RecipeTags)
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (recipe == null)
                return null;

            // Update basic properties
            recipe.Title = request.Title;
            recipe.Description = request.Description;
            recipe.Instructions = request.Instructions;
            recipe.PrepTimeMinutes = request.PrepTimeMinutes;
            recipe.CookTimeMinutes = request.CookTimeMinutes;
            recipe.Servings = request.Servings;
            recipe.Difficulty = request.Difficulty;
            recipe.ImageUrl = request.ImageUrl;
            recipe.CategoryId = request.CategoryId;
            recipe.UpdatedAt = DateTime.UtcNow;

            // Remove existing ingredients and tags
            _context.RecipeIngredients.RemoveRange(recipe.RecipeIngredients);
            _context.RecipeTags.RemoveRange(recipe.RecipeTags);

            // Add new ingredients
            foreach (var ingredientRequest in request.Ingredients)
            {
                var ingredient = await GetOrCreateIngredientAsync(ingredientRequest);
                
                var recipeIngredient = new RecipeIngredient
                {
                    RecipeId = recipe.Id,
                    IngredientId = ingredient.Id,
                    Quantity = ingredientRequest.Quantity,
                    Unit = ingredientRequest.Unit,
                    Notes = ingredientRequest.Notes
                };

                _context.RecipeIngredients.Add(recipeIngredient);
            }

            // Add new tags
            foreach (var tagId in request.TagIds)
            {
                var recipeTag = new RecipeTag
                {
                    RecipeId = recipe.Id,
                    TagId = tagId
                };
                _context.RecipeTags.Add(recipeTag);
            }

            await _context.SaveChangesAsync();

            return await GetRecipeByIdAsync(recipe.Id);
        }

        public async Task<bool> DeleteRecipeAsync(int id, int userId)
        {
            var recipe = await _context.Recipes
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (recipe == null)
                return false;

            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<RecipeDto>> GetUserRecipesAsync(int userId)
        {
            var recipes = await _context.Recipes
                .Include(r => r.User)
                .Include(r => r.Category)
                .Include(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .Include(r => r.RecipeTags)
                    .ThenInclude(rt => rt.Tag)
                .Include(r => r.RecipeRatings)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return recipes.Select(MapToRecipeDto);
        }

        private async Task<Ingredient> GetOrCreateIngredientAsync(CreateRecipeIngredient request)
        {
            Ingredient ingredient;

            if (request.IngredientId.HasValue)
            {
                ingredient = await _context.Ingredients.FindAsync(request.IngredientId.Value)
                    ?? throw new ArgumentException($"Ingredient with ID {request.IngredientId} not found");
            }
            else if (!string.IsNullOrEmpty(request.IngredientName))
            {
                ingredient = await _context.Ingredients
                    .FirstOrDefaultAsync(i => i.Name.ToLower() == request.IngredientName.ToLower());

                if (ingredient == null)
                {
                    ingredient = new Ingredient
                    {
                        Name = request.IngredientName,
                        Unit = request.Unit
                    };
                    _context.Ingredients.Add(ingredient);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                throw new ArgumentException("Either IngredientId or IngredientName must be provided");
            }

            return ingredient;
        }

        private RecipeDto MapToRecipeDto(Recipe recipe)
        {
            return new RecipeDto
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Description = recipe.Description,
                Instructions = recipe.Instructions,
                PrepTimeMinutes = recipe.PrepTimeMinutes,
                CookTimeMinutes = recipe.CookTimeMinutes,
                Servings = recipe.Servings,
                Difficulty = recipe.Difficulty,
                ImageUrl = recipe.ImageUrl,
                UserId = recipe.UserId,
                UserName = recipe.User.Username,
                CategoryId = recipe.CategoryId,
                CategoryName = recipe.Category?.Name,
                CreatedAt = recipe.CreatedAt,
                UpdatedAt = recipe.UpdatedAt,
                Ingredients = recipe.RecipeIngredients.Select(ri => new RecipeIngredientDto
                {
                    Id = ri.Id,
                    IngredientId = ri.IngredientId,
                    IngredientName = ri.Ingredient.Name,
                    Quantity = ri.Quantity,
                    Unit = ri.Unit,
                    Notes = ri.Notes
                }).ToList(),
                Tags = recipe.RecipeTags.Select(rt => new TagDto
                {
                    Id = rt.Tag.Id,
                    Name = rt.Tag.Name,
                    Color = rt.Tag.Color
                }).ToList(),
                AverageRating = recipe.RecipeRatings.Any() ? recipe.RecipeRatings.Average(r => r.Rating) : null,
                RatingCount = recipe.RecipeRatings.Count
            };
        }
    }
}