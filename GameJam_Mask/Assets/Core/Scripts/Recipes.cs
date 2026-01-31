using System;
using UnityEngine;

[Serializable]
public class Recipe
{
    public string aId;
    public string bId;
    public string resultMaskId;

    public Recipe(string aId, string bId, string resultMaskId)
    {
        this.aId = aId;
        this.bId = bId;
        this.resultMaskId = resultMaskId;
    }

    public bool Matches(string x, string y)
    {
        // 无序匹配：A+B 或 B+A 都算
        return (aId == x && bId == y) || (aId == y && bId == x);
    }
}
