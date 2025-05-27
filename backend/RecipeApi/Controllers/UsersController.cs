using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeApi.Data;
using RecipeApi.DTOs;
using System.Security.Claims;

namespace RecipeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly RecipeDbContext _context;

        public UsersController(RecipeDbContext context)
        {
            _context = context;
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetProfile()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var user = await _context.Users
                    .Where(u => u.Id == userId)
                    .Select(u => new UserDto
                    {
                        Id = u.Id,
                        Username = u.Username,
                        Email = u.Email,
                        CreatedAt = u.CreatedAt
                    })
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching user profile", error = ex.Message });
            }
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<ActionResult<UserDto>> UpdateProfile([FromBody] UpdateUserDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                // Check if username is already taken by another user
                if (request.Username != user.Username)
                {
                    var existingUser = await _context.Users
                        .FirstOrDefaultAsync(u => u.Username.ToLower() == request.Username.ToLower() && u.Id != userId);

                    if (existingUser != null)
                    {
                        return BadRequest(new { message = "Username is already taken" });
                    }
                }

                // Check if email is already taken by another user
                if (request.Email != user.Email)
                {
                    var existingUser = await _context.Users
                        .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower() && u.Id != userId);

                    if (existingUser != null)
                    {
                        return BadRequest(new { message = "Email is already taken" });
                    }
                }

                user.Username = request.Username;
                user.Email = request.Email;
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    CreatedAt = user.CreatedAt
                };

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating user profile", error = ex.Message });
            }
        }

        [HttpGet("{id}/recipes")]
        public async Task<ActionResult<IEnumerable<RecipeDto>>> GetUserRecipes(int id)
        {
            try
            {
                // Check if user exists
                var userExists = await _context.Users.AnyAsync(u => u.Id == id);
                if (!userExists)
                {
                    return NotFound(new { message = "User not found" });
                }

                var recipes = await _context.Recipes
                    .Where(r => r.UserId == id)
                    .Include(r => r.Category)
                    .Include(r => r.User)
                    .Include(r => r.RecipeTags)
                        .ThenInclude(rt => rt.Tag)
                    .Include(r => r.RecipeIngredients)
                        .ThenInclude(ri => ri.Ingredient)
                    .Select(r => new RecipeDto
                    {
                        Id = r.Id,
                        Title = r.Title,
                        Description = r.Description,
                        Instructions = r.Instructions,
                        PrepTimeMinutes = r.PrepTimeMinutes,
                        CookTimeMinutes = r.CookTimeMinutes,
                        Servings = r.Servings,
                        Difficulty = r.Difficulty,
                        ImageUrl = r.ImageUrl,
                        UserId = r.UserId,
                        UserName = r.User.Username,
                        CategoryId = r.CategoryId,
                        CategoryName = r.Category != null ? r.Category.Name : null,
                        CreatedAt = r.CreatedAt,
                        UpdatedAt = r.UpdatedAt,
                        Tags = r.RecipeTags.Select(rt => new TagDto
                        {
                            Id = rt.Tag.Id,
                            Name = rt.Tag.Name,
                            Color = rt.Tag.Color
                        }).ToList(),
                        Ingredients = r.RecipeIngredients.Select(ri => new RecipeIngredientDto
                        {
                            Id = ri.Id,
                            IngredientId = ri.IngredientId,
                            IngredientName = ri.Ingredient.Name,
                            Quantity = ri.Quantity,
                            Unit = ri.Unit,
                            Notes = ri.Notes
                        }).ToList()
                    })
                    .ToListAsync();

                return Ok(recipes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching user recipes", error = ex.Message });
            }
        }
    }
} 