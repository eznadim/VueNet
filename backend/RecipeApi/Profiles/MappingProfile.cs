using AutoMapper;
using RecipeApi.DTOs;
using RecipeApi.Models;

namespace RecipeApi.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mappings
            CreateMap<User, UserDto>();

            // Category mappings
            CreateMap<Category, CategoryDto>();

            // Tag mappings
            CreateMap<Tag, TagDto>();

            // Ingredient mappings
            CreateMap<Ingredient, IngredientDto>();

            // Recipe ingredient mappings
            CreateMap<RecipeIngredient, RecipeIngredientDto>()
                .ForMember(dest => dest.IngredientName, opt => opt.MapFrom(src => src.Ingredient.Name));

            // Recipe mappings
            CreateMap<Recipe, RecipeDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
                .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.RecipeIngredients))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.RecipeTags.Select(rt => rt.Tag)))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => src.RecipeRatings.Any() ? src.RecipeRatings.Average(r => r.Rating) : (double?)null))
                .ForMember(dest => dest.RatingCount, opt => opt.MapFrom(src => src.RecipeRatings.Count));
        }
    }
}