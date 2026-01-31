using System.Collections;
using UnityEngine;

public class FadeCanvasController : MonoBehaviour
{
    public static FadeCanvasController I { get; private set; }

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.3f;

    private Coroutine currentFade;

    private void Awake()
    {
        if (I != null)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(gameObject);

        if (canvasGroup == null)
            canvasGroup = GetComponentInChildren<CanvasGroup>();

        // 启动时默认全黑 -> 再淡入
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    private void Start()
    {
        FadeIn();
    }

    public void FadeIn()
    {
        StartFade(0f);
    }

    public void FadeOut()
    {
        StartFade(1f);
    }

    public void FadeOutInstant()
    {
        StopCurrentFade();
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    public void FadeInInstant()
    {
        StopCurrentFade();
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

    private void StartFade(float targetAlpha)
    {
        StopCurrentFade();
        currentFade = StartCoroutine(CoFade(targetAlpha));
    }

    private void StopCurrentFade()
    {
        if (currentFade != null)
            StopCoroutine(currentFade);
        currentFade = null;
    }

    private IEnumerator CoFade(float target)
    {
        float start = canvasGroup.alpha;
        float t = 0f;

        // FadeOut 时阻挡输入
        canvasGroup.blocksRaycasts = true;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, target, t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = target;

        // 完全透明后不再挡输入
        if (Mathf.Approximately(target, 0f))
            canvasGroup.blocksRaycasts = false;
    }
}
