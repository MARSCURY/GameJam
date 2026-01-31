using System.Collections.Generic;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public static StoryManager I { get; private set; }

    private HashSet<string> flags = new HashSet<string>();
    private Dictionary<string, string> choices = new Dictionary<string, string>();

    public int LoopIndex => SaveManager.I.Data.loopIndex;

    private void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SyncFromSave()
    {
        flags.Clear();
        choices.Clear();

        foreach (var f in SaveManager.I.Data.flags)
            flags.Add(f);

        foreach (var kv in SaveManager.I.Data.choices)
            choices[kv.key] = kv.value;
    }

    public void WriteBackToSave()
    {
        SaveManager.I.Data.flags = new List<string>(flags);

        var list = new List<StoryState.ChoiceKV>();
        foreach (var kv in choices)
            list.Add(new StoryState.ChoiceKV { key = kv.Key, value = kv.Value });

        SaveManager.I.Data.choices = list;
    }

    public bool HasFlag(string id) => flags.Contains(id);

    public void SetFlag(string id, bool value = true)
    {
        if (value) flags.Add(id);
        else flags.Remove(id);
    }

    public string GetChoice(string key, string defaultValue = "")
        => choices.TryGetValue(key, out var v) ? v : defaultValue;

    public void SetChoice(string key, string value)
        => choices[key] = value;

    public void NextLoop()
        => SaveManager.I.Data.loopIndex += 1;
}
