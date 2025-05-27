using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeApi.Data;
using RecipeApi.DTOs;
using RecipeApi.Models;
using System.Security.Claims;

namespace RecipeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RatingsController : ControllerBase
    {
        private readonly RecipeDbContext _context;

        public RatingsController(RecipeDbContext context)
        {
            _context = context;
        }

        [HttpGet("recipe/{recipeId}")]
        public async Task<ActionResult<IEnumerable<RecipeRatingDto>>> GetRecipeRatings(int recipeId)
        {
            try
            {
                var ratings = await _context.RecipeRatings
                    .Where(r => r.RecipeId == recipeId)
                    .Include(r => r.User)
                    .Select(r => new RecipeRatingDto
                    {
                        Id = r.Id,
                        RecipeId = r.RecipeId,
                        UserId = r.UserId,
                        Username = r.User.Username,
                        Rating = r.Rating,
                        Review = r.Review,
                        CreatedAt = r.CreatedAt
                    })
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();

                return Ok(ratings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching ratings", error = ex.Message });
            }
        }

        [HttpGet("recipe/{recipeId}/average")]
        public async Task<ActionResult<RecipeRatingAverageDto>> GetRecipeAverageRating(int recipeId)
        {
            try
            {
                var ratings = await _context.RecipeRatings
                    .Where(r => r.RecipeId == recipeId)
                    .ToListAsync();

                if (!ratings.Any())
                {
                    return Ok(new RecipeRatingAverageDto
                    {
                        RecipeId = recipeId,
                        AverageRating = 0,
                        TotalRatings = 0
                    });
                }

                var average = ratings.Average(r => r.Rating);
                var total = ratings.Count;

                return Ok(new RecipeRatingAverageDto
                {
                    RecipeId = recipeId,
                    AverageRating = Math.Round(average, 1),
                    TotalRatings = total
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while calculating average rating", error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<RecipeRatingDto>> CreateRating([FromBody] CreateRecipeRatingDto request)
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

                // Check if recipe exists
                var recipeExists = await _context.Recipes.AnyAsync(r => r.Id == request.RecipeId);
                if (!recipeExists)
                {
                    return NotFound(new { message = "Recipe not found" });
                }

                // Check if user already rated this recipe
                var existingRating = await _context.RecipeRatings
                    .FirstOrDefaultAsync(r => r.RecipeId == request.RecipeId && r.UserId == userId);

                if (existingRating != null)
                {
                    return BadRequest(new { message = "You have already rated this recipe. Use PUT to update your rating." });
                }

                var rating = new RecipeRating
                {
                    RecipeId = request.RecipeId,
                    UserId = userId,
                    Rating = request.Rating,
                    Review = request.Review
                };

                _context.RecipeRatings.Add(rating);
                await _context.SaveChangesAsync();

                // Load the user information for the response
                await _context.Entry(rating).Reference(r => r.User).LoadAsync();

                var ratingDto = new RecipeRatingDto
                {
                    Id = rating.Id,
                    RecipeId = rating.RecipeId,
                    UserId = rating.UserId,
                    Username = rating.User.Username,
                    Rating = rating.Rating,
                    Review = rating.Review,
                    CreatedAt = rating.CreatedAt
                };

                return CreatedAtAction(nameof(GetRecipeRatings), new { recipeId = rating.RecipeId }, ratingDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the rating", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<RecipeRatingDto>> UpdateRating(int id, [FromBody] UpdateRecipeRatingDto request)
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

                var rating = await _context.RecipeRatings
                    .Include(r => r.User)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (rating == null)
                {
                    return NotFound(new { message = "Rating not found" });
                }

                if (rating.UserId != userId)
                {
                    return Forbid("You can only update your own ratings");
                }

                rating.Rating = request.Rating;
                rating.Review = request.Review;

                await _context.SaveChangesAsync();

                var ratingDto = new RecipeRatingDto
                {
                    Id = rating.Id,
                    RecipeId = rating.RecipeId,
                    UserId = rating.UserId,
                    Username = rating.User.Username,
                    Rating = rating.Rating,
                    Review = rating.Review,
                    CreatedAt = rating.CreatedAt
                };

                return Ok(ratingDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the rating", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteRating(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var rating = await _context.RecipeRatings.FindAsync(id);
                if (rating == null)
                {
                    return NotFound(new { message = "Rating not found" });
                }

                if (rating.UserId != userId)
                {
                    return Forbid("You can only delete your own ratings");
                }

                _context.RecipeRatings.Remove(rating);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the rating", error = ex.Message });
            }
        }
    }
} 