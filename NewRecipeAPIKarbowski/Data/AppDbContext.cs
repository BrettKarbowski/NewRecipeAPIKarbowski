using Microsoft.EntityFrameworkCore;
using NewRecipeAPIKarbowski.Models.Entities;

namespace NewRecipeAPIKarbowski.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Recipe> Recipes { get; set; }
    }
}
