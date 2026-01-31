///如题目所示，这段代码的作用是加载场景，和FadeCanvasController（转场）有点联系）
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader I { get; private set; }

    [Header("Fade (Optional)")]
    [Tooltip("淡入淡出时长；如果场景里没有 FadeCanvasController，会自动忽略淡入淡出。")]
    [SerializeField] private float fadeDuration = 0.3f;

    public bool IsLoading { get; private set; }

    /// <summary>
    /// 可选：加载进度回调（0~1）。你现在不用也没关系。
    /// </summary>
    public event Action<float> OnProgress;

    private void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 切换场景（模式 B：玩家常驻）
    /// </summary>
    /// <param name="sceneName">目标场景名（必须在 Build Settings）</param>
    /// <param name="spawnId">目标落点 id（默认为 Default）</param>
    /// <param name="onLoaded">场景加载完成后回调（SpawnManager Start 之前/之后都可用，通常用来刷新UI）</param>
    public void Load(string sceneName, string spawnId = "Default", Action onLoaded = null)
    {
        if (IsLoading) return;
        StartCoroutine(CoLoad(sceneName, spawnId, onLoaded));
    }

    private IEnumerator CoLoad(string sceneName, string spawnId, Action onLoaded)
    {
        IsLoading = true;
        OnProgress?.Invoke(0f);

        // 0) 淡出（如果有 FadeCanvasController）
        var fader = FadeCanvasController.I;
        if (fader != null)
        {
            fader.FadeOut();
            yield return new WaitForSecondsRealtime(fadeDuration);
        }

        // 1) 写入存档位置
        SaveManager.I.Data.sceneName = sceneName;
        SaveManager.I.Data.spawnId = spawnId;

        // 2) 把 StoryManager 的 flags/choices 写回存档再保存
        StoryManager.I.WriteBackToSave();
        SaveManager.I.Save();

        // 3) 把 spawnId 交给新场景的 SpawnManager 使用
        SceneLoadRequest.NextSpawnId = spawnId;

        // 4) 异步加载
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        if (op == null)
        {
            Debug.LogError($"[SceneLoader] LoadSceneAsync failed: {sceneName}");
            IsLoading = false;
            yield break;
        }

        // Unity 的 op.progress 在 allowSceneActivation=true 时通常到 0.9 左右再跳完成
        while (!op.isDone)
        {
            // 简单归一化一下（可选）
            float p = Mathf.Clamp01(op.progress / 0.9f);
            OnProgress?.Invoke(p);
            yield return null;
        }

        // 等一帧：让新场景的 Awake/OnEnable 有机会先跑
        yield return null;

        // 5) 回调（用于 UI 刷新/重绑引用等）
        onLoaded?.Invoke();

        // 6) 淡入
        if (fader != null)
        {
            fader.FadeIn();
            // 不强制等待淡入结束，交互会更快恢复；你想等也可以加 WaitForSecondsRealtime
        }

        OnProgress?.Invoke(1f);
        IsLoading = false;
    }
}
