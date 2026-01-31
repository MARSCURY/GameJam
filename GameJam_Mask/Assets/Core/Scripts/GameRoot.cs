using UnityEngine;

public class GameRoot : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // 读档
        SaveManager.I.Load();
        // 同步剧情状态到运行时
        StoryManager.I.SyncFromSave();
    }
}
