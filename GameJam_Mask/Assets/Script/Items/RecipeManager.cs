using System;
using System.Collections.Generic;

namespace Items {
    public static class RecipeManager {
        private static readonly Dictionary<Item, Tuple<Item, Item>> Recipes = new();

        public static void AddRecipe(Item item1, Item item2, Item result) {
            Recipes.Add(item1, new Tuple<Item, Item>(item2, result));
            Recipes.Add(item2, new Tuple<Item, Item>(item1, result));
        }

        public static bool TryCraft(Item item1, Item item2, out Item result) {
            if (!Recipes.TryGetValue(item1, out Tuple<Item, Item> recipe) || recipe.Item1 != item2) {
                result = null;
                return false;
            }
            result = recipe.Item2;
            return true;
        }
    }
}