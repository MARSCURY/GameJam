using System.Collections.Generic;
using UnityEngine;

public class MaskDatabase : MonoBehaviour
{
    private readonly Dictionary<string, MaskDef> _masks = new Dictionary<string, MaskDef>();

    public void Add(MaskDef def) => _masks[def.id] = def;

    public bool TryGet(string id, out MaskDef def) => _masks.TryGetValue(id, out def);
}
