using System.Collections.Generic;
using UnityEngine;

public class RecipeBook : MonoBehaviour
{
    private readonly List<Recipe> _recipes = new List<Recipe>();

    public void Add(Recipe r) => _recipes.Add(r);

    public bool TryCraft(string aThoughtId, string bThoughtId, out string maskId)
    {
        foreach (var r in _recipes)
        {
            if (r.Matches(aThoughtId, bThoughtId))
            {
                maskId = r.resultMaskId;
                return true;
            }
        }
        maskId = null;
        return false;
    }
}
