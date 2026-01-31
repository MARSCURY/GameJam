using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RuntimeUI : MonoBehaviour
{
    private ThoughtInventory _inv;
    private CraftSystem _craft;
    private MaskSystem _mask;
    private GameState _state;
    private SubtitleSystem _subs;

    private RectTransform _thoughtListRoot;
    private Text _loopText;
    private Text _maskText;

    private Thought _selectedA;
    private Thought _selectedB;

    public void Init(ThoughtInventory inv, CraftSystem craft, MaskSystem mask, GameState state, SubtitleSystem subs)
    {
        _inv = inv;
        _craft = craft;
        _mask = mask;
        _state = state;
        _subs = subs;

        BuildUI();
        Refresh();
    }

    private void BuildUI()
    {
        // EventSystem
        if (FindObjectOfType<EventSystem>() == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }

        // Canvas
        var canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        // Background panel right
        var rightPanel = CreatePanel(canvasGO.transform, new Vector2(1, 0), new Vector2(1, 1), new Vector2(320, 0), new Vector2(-10, 0));
        rightPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0.35f);

        // Loop text
        _loopText = CreateText(rightPanel.transform, "LoopText", new Vector2(0, 1), new Vector2(1, 1), new Vector2(-20, 40), new Vector2(0, -10), 18, TextAnchor.MiddleLeft);

        // Mask text
        _maskText = CreateText(rightPanel.transform, "MaskText", new Vector2(0, 1), new Vector2(1, 1), new Vector2(-20, 40), new Vector2(0, -55), 16, TextAnchor.MiddleLeft);

        // Thought list root
        var listPanel = CreatePanel(rightPanel.transform, new Vector2(0, 0), new Vector2(1, 1), new Vector2(-20, -220), new Vector2(0, -110));
        listPanel.GetComponent<Image>().color = new Color(1, 1, 1, 0.05f);
        _thoughtListRoot = listPanel.GetComponent<RectTransform>();

        // Buttons
        var btnRow = CreatePanel(rightPanel.transform, new Vector2(0, 0), new Vector2(1, 0), new Vector2(-20, 200), new Vector2(0, 10));
        btnRow.GetComponent<Image>().color = new Color(0, 0, 0, 0);

        CreateButton(btnRow.transform, "合成(A+B)", new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 45), new Vector2(0, -10),
            () => { _craft.TryCraft(_selectedA, _selectedB); _selectedA = _selectedB = null; Refresh(); });

        CreateButton(btnRow.transform, "对妻子使用", new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 45), new Vector2(0, -60),
            () => { _mask.UseMaskOn(MaskTargetType.Wife); Refresh(); });

        CreateButton(btnRow.transform, "对女儿使用", new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 45), new Vector2(0, -110),
            () => { _mask.UseMaskOn(MaskTargetType.Daughter); Refresh(); });

        CreateButton(btnRow.transform, "进入下一轮", new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 45), new Vector2(0, -160),
            () => { _state.NextLoop(); _inv.Clear(); _selectedA = _selectedB = null; _subs.Show($"进入第 {_state.loopIndex} 轮"); Refresh(); });

        // Subtitle area bottom center
        var subtitlePanel = CreatePanel(canvasGO.transform, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(900, 80), new Vector2(0, 40));
        subtitlePanel.GetComponent<Image>().color = new Color(0, 0, 0, 0.45f);
        var subtitleText = CreateText(subtitlePanel.transform, "Subtitle", new Vector2(0, 0), new Vector2(1, 1), new Vector2(-20, -10), Vector2.zero, 18, TextAnchor.MiddleCenter);
        _subs.Init(subtitleText);
    }

    private void Refresh()
    {
        if (_loopText != null) _loopText.text = $"循环：第 {_state.loopIndex} 轮   路径锁：{_state.pathLocked}";
        if (_maskText != null) _maskText.text = _mask.currentMask == null ? "面具：<无>" : $"面具：{_mask.currentMask.displayName}（目标：{_mask.currentMask.targetType}）";

        // 清理旧条目
        for (int i = _thoughtListRoot.childCount - 1; i >= 0; i--) Destroy(_thoughtListRoot.GetChild(i).gameObject);

        // 生成念头按钮列表
        float y = -10;
        foreach (var t in _inv.thoughts)
        {
            var btn = CreateButton(_thoughtListRoot, $"• {t.displayName}", new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0, 40), new Vector2(0, y),
                () => OnClickThought(t));

            var img = btn.GetComponent<Image>();
            if (t == _selectedA || t == _selectedB) img.color = new Color(1, 1, 1, 0.35f);
            else img.color = new Color(1, 1, 1, 0.15f);

            y -= 45;
        }

        // 选中提示
        if (_selectedA != null || _selectedB != null)
        {
            string a = _selectedA?.displayName ?? "空";
            string b = _selectedB?.displayName ?? "空";
            _subs.Show($"已选择：A={a}  B={b}", 1.2f);
        }
    }

    private void OnClickThought(Thought t)
    {
        if (_selectedA == null) _selectedA = t;
        else if (_selectedB == null && t != _selectedA) _selectedB = t;
        else
        {
            // 第三个点击：替换B
            if (t != _selectedA) _selectedB = t;
        }
        Refresh();
    }

    // ---- UI helpers ----
    private GameObject CreatePanel(Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 sizeDelta, Vector2 anchoredPos)
    {
        var go = new GameObject("Panel", typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.sizeDelta = sizeDelta;
        rt.anchoredPosition = anchoredPos;
        return go;
    }

    private Text CreateText(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 sizeDelta, Vector2 anchoredPos, int fontSize, TextAnchor anchor)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.sizeDelta = sizeDelta;
        rt.anchoredPosition = anchoredPos;

        var t = go.GetComponent<Text>();
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = fontSize;
        t.alignment = anchor;
        t.color = Color.white;
        t.text = "";
        return t;
    }

    private GameObject CreateButton(Transform parent, string label, Vector2 anchorMin, Vector2 anchorMax, Vector2 sizeDelta, Vector2 anchoredPos, UnityEngine.Events.UnityAction onClick)
    {
        var go = new GameObject("Button", typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);

        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.sizeDelta = sizeDelta;
        rt.anchoredPosition = anchoredPos;

        var img = go.GetComponent<Image>();
        img.color = new Color(1, 1, 1, 0.15f);

        var btn = go.GetComponent<Button>();
        btn.onClick.AddListener(onClick);

        var text = CreateText(go.transform, "Label", new Vector2(0, 0), new Vector2(1, 1), new Vector2(-16, -8), Vector2.zero, 16, TextAnchor.MiddleLeft);
        text.text = label;
        text.color = Color.white;

        return go;
    }
}
