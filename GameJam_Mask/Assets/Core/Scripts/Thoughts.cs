using System;
using UnityEngine;

[Serializable]
public class Thought
{
    public string id;
    public string displayName;
    [TextArea] public string description;

    public Thought(string id, string name, string desc = "")
    {
        this.id = id;
        this.displayName = name;
        this.description = desc;
    }
}
