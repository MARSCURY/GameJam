using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager I { get; private set; }

    public StoryState Data { get; private set; } = new StoryState();

    private string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    private void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool HasSave() => File.Exists(SavePath);

    public void NewGame(string firstScene)
    {
        Data = new StoryState
        {
            loopIndex = 1,
            sceneName = firstScene,
            spawnId = "Default",
        };
        Save();
    }

    public void Load()
    {
        if (!HasSave())
        {
            Debug.LogWarning("[SaveManager] No save file, using default.");
            Data = new StoryState();
            return;
        }

        string json = File.ReadAllText(SavePath);
        Data = JsonUtility.FromJson<StoryState>(json) ?? new StoryState();
        Debug.Log("[SaveManager] Loaded.");
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(Data, true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"[SaveManager] Saved: {SavePath}");
    }
}
