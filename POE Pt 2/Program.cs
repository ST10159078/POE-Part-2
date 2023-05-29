using POE_Pt_2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace POE_Pt_2
{
    // Class representing an ingredient in a recipe
    class Ingredient
    {
        public string Name { get; set; }
        public string Quantity { get; set; }
        public string Unit { get; set; }
        public int Calories { get; set; }
        public FoodGroup Group { get; set; }
    }

    // Enumeration representing different food groups
    enum FoodGroup
    {
        Grains,
        Fruits,
        Vegetables,
        Proteins,
        Dairy,
        Fats
    }

    // Class representing a recipe
    class Recipe
    {
        public string Name { get; set; }
        public List<Ingredient> Ingredients { get; set; }
        public List<string> Steps { get; set; }
        private List<double> originalQuantities;

        public Recipe()
        {
            Ingredients = new List<Ingredient>();
            Steps = new List<string>();
            originalQuantities = new List<double>();
        }

        // Method to enter details of a recipe
        public void EnterRecipe()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("Enter the name of the recipe: ");
            Name = Console.ReadLine();

            Console.Write("Enter the number of ingredients: ");
            int ingredientCount = Convert.ToInt32(Console.ReadLine());

            for (int i = 0; i < ingredientCount; i++)
            {
                Console.WriteLine($"Enter ingredients {i + 1}:");
                Console.Write("Ingredient Name: ");
                string name = Console.ReadLine();
                Console.Write("Ingredient Quantity: ");
                string quantity = Console.ReadLine();
                Console.Write("Unit of Measurement(L, KG, ml, etc.): ");
                string unit = Console.ReadLine();
                Console.Write("Calories: ");
                int calories = Convert.ToInt32(Console.ReadLine());
                Console.Write("Food Group\n" +
                    "NB must be one of these (Grains, Fruits, Vegetables, Proteins, Dairy, Fats): ");
                string group = Console.ReadLine();

                double originalQuantity = Convert.ToDouble(quantity);
                originalQuantities.Add(originalQuantity);

                Ingredient ingredient = new Ingredient
                {
                    Name = name,
                    Quantity = quantity,
                    Unit = unit,
                    Calories = calories,
                    Group = (FoodGroup)Enum.Parse(typeof(FoodGroup), group, true)
                };

                Ingredients.Add(ingredient);
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("\nEnter the number of steps: ");
            int stepCount = Convert.ToInt32(Console.ReadLine());

            for (int i = 0; i < stepCount; i++)
            {
                Console.WriteLine($"Enter step {i + 1}:");
                string description = Console.ReadLine();
                Steps.Add(description);
            }
        }

        // Method to display the recipe details
        public void DisplayRecipe()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\nRecipe: {Name}");
            Console.WriteLine("Ingredients:");
            foreach (Ingredient ingredient in Ingredients)
            {
                Console.WriteLine($"- Ingredient name: {ingredient.Name} \n- {ingredient.Quantity} {ingredient.Unit} \n- Food group: {ingredient.Group} \n- ({ingredient.Calories} calories) ");
            }

            Console.WriteLine("\nSteps:");
            for (int i = 0; i < Steps.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {Steps[i]}");
            }
        }

        // Method to scale the recipe by a given factor
        public void ScaleRecipe(double factor)
        {
            foreach (Ingredient ingredient in Ingredients)
            {
                double originalQuantity = Convert.ToDouble(ingredient.Quantity);
                double scaledQuantity = originalQuantity * factor;
                ingredient.Quantity = scaledQuantity.ToString();
            }
        }

        // Method to reset the quantities of ingredients to the original values
        public void ResetQuantities()
        {
            if (originalQuantities.Count == Ingredients.Count)
            {
                for (int i = 0; i < Ingredients.Count; i++)
                {
                    Ingredient ingredient = Ingredients[i];
                    double originalQuantity = originalQuantities[i];
                    ingredient.Quantity = originalQuantity.ToString();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Original quantities not found for all ingredients.");
            }
        }

        // Method to calculate the total calories of the recipe
        public int CalculateTotalCalories()
        {
            int totalCalories = 0;
            foreach (Ingredient ingredient in Ingredients)
            {
                totalCalories += ingredient.Calories;
            }
            return totalCalories;
        }
    }


    // Class representing the recipe application
    class RecipeApplication
    {
        private List<Recipe> recipes;

        public RecipeApplication()
        {
            recipes = new List<Recipe>();
        }

        // Method to add a recipe to the application
        public void AddRecipe(Recipe recipe)
        {
            recipes.Add(recipe);
            recipe.DisplayRecipe();
            int totalCalories = recipe.CalculateTotalCalories();
            if (totalCalories > 300)
            {
                RecipeExceededCalories?.Invoke(recipe.Name);
            }
        }

        // Method to display the list of recipes in alphabetical order by name
        public void DisplayRecipeList()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nRecipe List:");
            recipes.Sort((r1, r2) => string.Compare(r1.Name, r2.Name, StringComparison.OrdinalIgnoreCase));
            foreach (Recipe recipe in recipes)
            {
                Console.WriteLine(recipe.Name);
            }
        }

        // Method to display a recipe by its name
        public void DisplayRecipeByName(string name)
        {
            Recipe recipe = recipes.FirstOrDefault(r => string.Equals(r.Name, name, StringComparison.OrdinalIgnoreCase));
            if (recipe != null)
            {
                recipe.DisplayRecipe();
                int totalCalories = recipe.CalculateTotalCalories();
                if (totalCalories > 300)
                {
                    RecipeExceededCalories?.Invoke(recipe.Name);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"Recipe '{name}' not found.");
            }
        }

        // Method to scale a recipe by its name
        public void ScaleRecipeByName(string name, double factor)
        {
            Recipe recipe = recipes.FirstOrDefault(r => string.Equals(r.Name, name, StringComparison.OrdinalIgnoreCase));
            if (recipe != null)
            {
                recipe.ScaleRecipe(factor);
                recipe.DisplayRecipe();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"Recipe '{name}' not found.");
            }
        }

        // Method to reset the quantities of a recipe by its name
        public void ResetQuantitiesByName(string name)
        {
            Recipe recipe = recipes.FirstOrDefault(r => string.Equals(r.Name, name, StringComparison.OrdinalIgnoreCase));
            if (recipe != null)
            {
                recipe.ResetQuantities();
                recipe.DisplayRecipe();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"Recipe '{name}' not found.");
            }
        }

        // Method to clear all recipes
        public void ClearRecipes()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            recipes.Clear();
            Console.WriteLine("All recipes cleared.");
        }

        // Event to notify when a recipe exceeds 300 calories
        public event Action<string> RecipeExceededCalories;

        // Method to run the recipe application
        public void RunApplication()
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("\n----- Recipe Application -----");
                Console.WriteLine("1. Enter Recipe");
                Console.WriteLine("2. Display Recipe List");
                Console.WriteLine("3. Display Recipe by Name");
                Console.WriteLine("4. Scale Recipe");
                Console.WriteLine("5. Reset Quantities");
                Console.WriteLine("6. Clear Recipe");
                Console.WriteLine("7. Exit");
                Console.Write("Enter your choice (1-7): ");

                int choice = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine();

                switch (choice)
                {
                    case 1:
                        Recipe newRecipe = new Recipe();
                        newRecipe.EnterRecipe();
                        AddRecipe(newRecipe);
                        break;
                    case 2:
                        DisplayRecipeList();
                        break;
                    case 3:
                        Console.Write("\nEnter the name of the recipe to display: ");
                        string recipeName = Console.ReadLine();
                        DisplayRecipeByName(recipeName);
                        break;
                    case 4:
                        Console.Write("Enter the name of the recipe to scale: ");
                        string recipeToScale = Console.ReadLine();
                        Console.Write("Enter scaling factor (0.5, 2, or 3): ");
                        double factor = Convert.ToDouble(Console.ReadLine());
                        ScaleRecipeByName(recipeToScale, factor);
                        break;
                    case 5:
                        Console.Write("Enter the name of the recipe to reset quantities: ");
                        string recipeToReset = Console.ReadLine();
                        ResetQuantitiesByName(recipeToReset);
                        break;
                    case 6:
                        ClearRecipes();
                        break;
                    case 7:
                        Console.WriteLine("Exiting...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            RecipeApplication application = new RecipeApplication();
            application.RecipeExceededCalories += (name) => Console.WriteLine($"Warning: Recipe '{name}' exceeds 300 calories.");

            application.RunApplication();
        }
    }
}
