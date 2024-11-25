using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewRecipeAPIKarbowski.Data;
using NewRecipeAPIKarbowski.Models;
using NewRecipeAPIKarbowski.Models.Entities;
using System.Linq.Expressions;
using System.Text.Json;

namespace NewRecipeAPIKarbowski.Controllers
{
    // localhost:xxxx/api/recipes
    [Route("api/[controller]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private readonly AppDbContext dbContext;
        private readonly ILogger<RecipesController> _logger;

        public RecipesController(AppDbContext dbContext, ILogger<RecipesController> logger)
        {
            this.dbContext = dbContext;
            _logger = logger;
        }

        [HttpPost]
        [Route("seed-from-json")]
        public IActionResult SeedDataFromJason()
        {
            try
            {
                _logger.LogInformation("Seeding database from JSON file.");
                var jsonFilePath = Path.Combine(AppContext.BaseDirectory, "Data", "recipes.json");

                if (!System.IO.File.Exists(jsonFilePath))
                {
                    _logger.LogWarning("JSON file not found: {FilePath}", jsonFilePath);
                    return NotFound($"JSON file not found at {jsonFilePath}");
                }

                var jsonData = System.IO.File.ReadAllText(jsonFilePath);
                var recipes = JsonSerializer.Deserialize<List<Recipe>>(jsonData);

                if (recipes == null || !recipes.Any())
                {
                    _logger.LogWarning("No recipes found in the JSON file.");
                    return BadRequest("No recipes found in the JSON file.");
                }

                dbContext.Recipes.AddRange(recipes);
                dbContext.SaveChanges();

                _logger.LogInformation("Database seeded successfully with {Count} recipes.", recipes.Count);
                return Ok(new { Message = "Database seeded successfully", recipes.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while seeding the database from JSON.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        public IActionResult GetAllRecipes()
        {
            try
            {
                _logger.LogInformation("Fetching all recipes.");
                var allRecipes = dbContext.Recipes.ToList();

                return Ok(allRecipes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while fetching all recipes.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("{id:guid}")]
        public IActionResult GetRecipeById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching recipe with ID: {RecipeId}", id);
                var recipe = dbContext.Recipes.Find(id);

                if (recipe is null)
                {
                    _logger.LogWarning("Recipe with ID {RecipeId} not found", id);
                    return NotFound();
                }

                return Ok(recipe);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while fetching recipe with ID: {RecipeId}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult AddRecipe(AddRecipeDTO addRecipeDTO)
        {
            try
            {
                _logger.LogInformation("Adding a new recipe: {RecipeName}", addRecipeDTO.Name);
                var recipeEntity = new Recipe()
                {
                    Name = addRecipeDTO.Name,
                    ImageURL = addRecipeDTO.ImageURL,
                    Time = addRecipeDTO.Time,
                    Description = addRecipeDTO.Description,
                    Ingredients = addRecipeDTO.Ingredients,
                    Directions = addRecipeDTO.Directions
                };

                dbContext.Recipes.Add(recipeEntity);
                dbContext.SaveChanges();

                _logger.LogInformation("Recipe added successfully with ID: {RecipeId}", recipeEntity.Id);
                return Ok(recipeEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while adding a new recipe.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut]
        [Route("{id:guid}")]
        public IActionResult UpdateRecipe(Guid id, UpdateRecipeDTO updateRecipeDTO)
        {
            try
            {
                _logger.LogInformation("Updating recipe with ID: {RecipeId}", id);
                var recipe = dbContext.Recipes.Find(id);

                if (recipe is null)
                {
                    _logger.LogWarning("Recipe with ID {RecipeId} not found", id);
                    return NotFound();
                }

                recipe.Name = updateRecipeDTO.Name;
                recipe.ImageURL = updateRecipeDTO.ImageURL;
                recipe.Time = updateRecipeDTO.Time;
                recipe.Description = updateRecipeDTO.Description;
                recipe.Ingredients = updateRecipeDTO.Ingredients;
                recipe.Directions = updateRecipeDTO.Directions;

                dbContext.SaveChanges();

                _logger.LogInformation("Recipe with ID {RecipeId} updated successfully", id);
                return Ok(recipe);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while updating recipe with ID: {RecipeId}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public IActionResult DeleteRecipe(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting recipe with ID: {RecipeId}", id);
                var recipe = dbContext.Recipes.Find(id);

                if (recipe is null)
                {
                    _logger.LogWarning("Recipe with ID {RecipeId} not found", id);
                    return NotFound();
                }

                dbContext.Recipes.Remove(recipe);
                dbContext.SaveChanges();

                _logger.LogInformation("Recipe with ID {RecipeId} deleted successfully", id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while deleting recipe with ID: {RecipeId}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
