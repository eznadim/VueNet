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
    public class TagsController : ControllerBase
    {
        private readonly RecipeDbContext _context;

        public TagsController(RecipeDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetTags()
        {
            try
            {
                var tags = await _context.Tags
                    .Select(t => new TagDto { Id = t.Id, Name = t.Name, Color = t.Color })
                    .ToListAsync();
                
                return Ok(tags);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching tags", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TagDto>> GetTag(int id)
        {
            try
            {
                var tag = await _context.Tags
                    .Where(t => t.Id == id)
                    .Select(t => new TagDto { Id = t.Id, Name = t.Name, Color = t.Color })
                    .FirstOrDefaultAsync();

                if (tag == null)
                {
                    return NotFound(new { message = "Tag not found" });
                }

                return Ok(tag);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the tag", error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<TagDto>> CreateTag([FromBody] CreateTagDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var tag = new Tag
                {
                    Name = request.Name,
                    Color = request.Color ?? "#007bff"
                };

                _context.Tags.Add(tag);
                await _context.SaveChangesAsync();

                var tagDto = new TagDto
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    Color = tag.Color
                };

                return CreatedAtAction(nameof(GetTag), new { id = tag.Id }, tagDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the tag", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<TagDto>> UpdateTag(int id, [FromBody] UpdateTagDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var tag = await _context.Tags.FindAsync(id);
                if (tag == null)
                {
                    return NotFound(new { message = "Tag not found" });
                }

                tag.Name = request.Name;
                tag.Color = request.Color ?? "#007bff";

                await _context.SaveChangesAsync();

                var tagDto = new TagDto
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    Color = tag.Color
                };

                return Ok(tagDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the tag", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteTag(int id)
        {
            try
            {
                var tag = await _context.Tags.FindAsync(id);
                if (tag == null)
                {
                    return NotFound(new { message = "Tag not found" });
                }

                // Check if tag is being used by any recipes
                var hasRecipes = await _context.RecipeTags.AnyAsync(rt => rt.TagId == id);
                if (hasRecipes)
                {
                    return BadRequest(new { message = "Cannot delete tag that is being used by recipes" });
                }

                _context.Tags.Remove(tag);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the tag", error = ex.Message });
            }
        }
    }
} 