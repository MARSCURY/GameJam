using System.Collections.Generic;
using UnityEngine;

public class ThoughtInventory : MonoBehaviour
{
    public readonly List<Thought> thoughts = new List<Thought>();

    public void Add(Thought t)
    {
        thoughts.Add(t);
    }

    public void Remove(Thought t)
    {
        thoughts.Remove(t);
    }

    public void Clear()
    {
        thoughts.Clear();
    }
}
