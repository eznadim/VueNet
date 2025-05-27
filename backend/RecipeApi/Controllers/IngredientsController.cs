using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeApi.Data;
using RecipeApi.DTOs;
using RecipeApi.Models;

namespace RecipeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IngredientsController : ControllerBase
    {
        private readonly RecipeDbContext _context;

        public IngredientsController(RecipeDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<IngredientDto>>> GetIngredients([FromQuery] string? searchTerm = null)
        {
            try
            {
                var query = _context.Ingredients.AsQueryable();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(i => i.Name.Contains(searchTerm));
                }

                var ingredients = await query
                    .Select(i => new IngredientDto { Id = i.Id, Name = i.Name, Unit = i.Unit })
                    .ToListAsync();
                
                return Ok(ingredients);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching ingredients", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IngredientDto>> GetIngredient(int id)
        {
            try
            {
                var ingredient = await _context.Ingredients
                    .Where(i => i.Id == id)
                    .Select(i => new IngredientDto { Id = i.Id, Name = i.Name, Unit = i.Unit })
                    .FirstOrDefaultAsync();

                if (ingredient == null)
                {
                    return NotFound(new { message = "Ingredient not found" });
                }

                return Ok(ingredient);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the ingredient", error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<IngredientDto>> CreateIngredient([FromBody] CreateIngredientDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check if ingredient already exists
                var existingIngredient = await _context.Ingredients
                    .FirstOrDefaultAsync(i => i.Name.ToLower() == request.Name.ToLower());

                if (existingIngredient != null)
                {
                    return BadRequest(new { message = "An ingredient with this name already exists" });
                }

                var ingredient = new Ingredient
                {
                    Name = request.Name,
                    Unit = request.Unit
                };

                _context.Ingredients.Add(ingredient);
                await _context.SaveChangesAsync();

                var ingredientDto = new IngredientDto
                {
                    Id = ingredient.Id,
                    Name = ingredient.Name,
                    Unit = ingredient.Unit
                };

                return CreatedAtAction(nameof(GetIngredient), new { id = ingredient.Id }, ingredientDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the ingredient", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<IngredientDto>> UpdateIngredient(int id, [FromBody] UpdateIngredientDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var ingredient = await _context.Ingredients.FindAsync(id);
                if (ingredient == null)
                {
                    return NotFound(new { message = "Ingredient not found" });
                }

                // Check if another ingredient with the same name exists
                var existingIngredient = await _context.Ingredients
                    .FirstOrDefaultAsync(i => i.Name.ToLower() == request.Name.ToLower() && i.Id != id);

                if (existingIngredient != null)
                {
                    return BadRequest(new { message = "An ingredient with this name already exists" });
                }

                ingredient.Name = request.Name;
                ingredient.Unit = request.Unit;

                await _context.SaveChangesAsync();

                var ingredientDto = new IngredientDto
                {
                    Id = ingredient.Id,
                    Name = ingredient.Name,
                    Unit = ingredient.Unit
                };

                return Ok(ingredientDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the ingredient", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteIngredient(int id)
        {
            try
            {
                var ingredient = await _context.Ingredients.FindAsync(id);
                if (ingredient == null)
                {
                    return NotFound(new { message = "Ingredient not found" });
                }

                // Check if ingredient is being used by any recipes
                var hasRecipes = await _context.RecipeIngredients.AnyAsync(ri => ri.IngredientId == id);
                if (hasRecipes)
                {
                    return BadRequest(new { message = "Cannot delete ingredient that is being used by recipes" });
                }

                _context.Ingredients.Remove(ingredient);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the ingredient", error = ex.Message });
            }
        }
    }
} 