using UnityEngine;

public class MenuController : MonoBehaviour
{
    [Header("First Scene")]
    public string firstScene = "Train_Intro";
    public string firstSpawnId = "Default";

    private void Start()
    {
        EnterMenuMode();
    }

    /* ================= Buttons ================= */

    public void NewGame()
    {
        if (!CheckSystems()) return;

        ExitMenuMode();

        SaveManager.I.NewGame(firstScene);
        StoryManager.I.SyncFromSave();
        SceneLoader.I.Load(firstScene, firstSpawnId);
    }

    public void Continue()
    {
        if (!CheckSystems()) return;

        ExitMenuMode();

        SaveManager.I.Load();
        StoryManager.I.SyncFromSave();

        string scene = SaveManager.I.Data.sceneName;
        string spawn = SaveManager.I.Data.spawnId;

        if (string.IsNullOrEmpty(scene)) scene = firstScene;
        if (string.IsNullOrEmpty(spawn)) spawn = firstSpawnId;

        SceneLoader.I.Load(scene, spawn);
    }

    public void Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    /* ================= Mode Switch ================= */

    private void EnterMenuMode()
    {
        // 解锁鼠标
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 禁用第一人称输入（模式B：玩家常驻）
        SetFPSControl(false);
    }

    private void ExitMenuMode()
    {
        // 启用第一人称输入
        SetFPSControl(true);

        // 锁回鼠标
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void SetFPSControl(bool enabled)
    {
        var player = GameObject.FindWithTag("Player");
        if (player == null) return;

        var fps = player.GetComponent<SimpleFPSController>();
        if (fps != null) fps.enabled = enabled;
    }

    private bool CheckSystems()
    {
        if (SaveManager.I == null)
        {
            Debug.LogError("[Menu] SaveManager is NULL (did you start from Boot?)");
            return false;
        }

        if (StoryManager.I == null)
        {
            Debug.LogError("[Menu] StoryManager is NULL");
            return false;
        }

        if (SceneLoader.I == null)
        {
            Debug.LogError("[Menu] SceneLoader is NULL");
            return false;
        }

        return true;
    }
}
