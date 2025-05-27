using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeApi.DTOs;
using RecipeApi.Services;
using System.Security.Claims;
using System.Linq;

namespace RecipeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipesController : ControllerBase
    {
        private readonly IRecipeService _recipeService;

        public RecipesController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecipeDto>>> GetRecipes(
            [FromQuery] int? categoryId = null,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var recipes = await _recipeService.GetAllRecipesAsync(categoryId, searchTerm, page, pageSize);
                return Ok(recipes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching recipes", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RecipeDto>> GetRecipe(int id)
        {
            try
            {
                var recipe = await _recipeService.GetRecipeByIdAsync(id);
                if (recipe == null)
                {
                    return NotFound(new { message = "Recipe not found" });
                }
                return Ok(recipe);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the recipe", error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<RecipeDto>> CreateRecipe([FromBody] CreateRecipe request)
        {
            try
            {
                // Check model validation
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => new { 
                            Field = x.Key, 
                            Errors = x.Value.Errors.Select(e => e.ErrorMessage) 
                        })
                        .ToArray();
                    
                    return BadRequest(new { 
                        message = "Validation failed", 
                        errors = errors 
                    });
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var recipe = await _recipeService.CreateRecipeAsync(request, userId);
                return CreatedAtAction(nameof(GetRecipe), new { id = recipe.Id }, recipe);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the recipe", error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<RecipeDto>> UpdateRecipe(int id, [FromBody] UpdateRecipe request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var recipe = await _recipeService.UpdateRecipeAsync(id, request, userId);
                if (recipe == null)
                {
                    return NotFound(new { message = "Recipe not found or you don't have permission to update it" });
                }

                return Ok(recipe);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the recipe", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteRecipe(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var success = await _recipeService.DeleteRecipeAsync(id, userId);
                if (!success)
                {
                    return NotFound(new { message = "Recipe not found or you don't have permission to delete it" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the recipe", error = ex.Message });
            }
        }

        [HttpGet("my-recipes")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<RecipeDto>>> GetMyRecipes()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var recipes = await _recipeService.GetUserRecipesAsync(userId);
                return Ok(recipes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching your recipes", error = ex.Message });
            }
        }
    }
} 