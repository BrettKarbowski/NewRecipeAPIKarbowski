namespace NewRecipeAPIKarbowski.Models.Entities
{
    public class Recipe
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public required string ImageURL { get; set; }

        public required string Time {  get; set; }

        public required string Description { get; set; }

        public required string Ingredients { get; set; }

        public required string Directions { get; set; }
    }
}
