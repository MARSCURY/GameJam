using UnityEngine;

// 挂在“可被拿起悬停”的世界物体上（你的车票/存折/道具模型）
public class HoldableWorldObject : MonoBehaviour
{
    [Header("文案/展示")]
    public string title = "物品";
    [TextArea] public string description = "物品描述...（用于底部字幕）";

    [Header("第二次E确认行为")]
    [Tooltip("true=确认后入栏位并隐藏世界物体；false=确认后回到原位")]
    public bool confirmStoreIntoInventory = true;

    [Header("悬停位置（相对摄像机）")]
    public Vector3 holdLocalOffset = new Vector3(0.3f, -0.2f, 0.8f);
    public Vector3 holdLocalEuler = Vector3.zero;

    private Transform _originParent;
    private Vector3 _originPos;
    private Quaternion _originRot;

    private Rigidbody _rb;

    // ✅ 关键：缓存所有 colliders（包含子物体）
    private Collider[] _allCols;

    public bool IsHeld { get; private set; }

    private void Awake()
    {
        _originParent = transform.parent;
        _originPos = transform.position;
        _originRot = transform.rotation;

        _rb = GetComponent<Rigidbody>();

        // 包含子物体；即使你以后换模型/加Collider也不怕漏
        _allCols = GetComponentsInChildren<Collider>(true);
        if (_allCols == null) _allCols = new Collider[0];
    }

    public void BeginHold(Transform cameraTransform)
    {
        if (IsHeld) return;
        IsHeld = true;

        // 关物理
        if (_rb != null)
        {
            _rb.isKinematic = true;
            _rb.useGravity = false;
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }

        // ✅ 关所有碰撞，避免挡射线/卡角色
        SetCollidersEnabled(false);

        transform.SetParent(cameraTransform, true);
        transform.localPosition = holdLocalOffset;
        transform.localRotation = Quaternion.Euler(holdLocalEuler);
    }

    public void EndHold_StoreAndHide()
    {
        IsHeld = false;

        // 入栏位后：世界物体隐藏（最省事）
        gameObject.SetActive(false);
        // 注意：隐藏后 collider 状态无所谓；下次 DropAt 会统一恢复
    }

    public void EndHold_ReturnToOrigin()
    {
        IsHeld = false;

        transform.SetParent(_originParent, true);
        transform.position = _originPos;
        transform.rotation = _originRot;

        // ✅ 恢复碰撞
        SetCollidersEnabled(true);

        // 物理恢复
        if (_rb != null)
        {
            _rb.isKinematic = false;
            _rb.useGravity = true;
        }
    }

    // 给“扔下”用：从玩家前方生成并自然掉落
    public void DropAt(Vector3 worldPos)
    {
        IsHeld = false;

        gameObject.SetActive(true);
        transform.SetParent(null, true);
        transform.position = worldPos;

        // ✅ 恢复碰撞
        SetCollidersEnabled(true);

        // 物理恢复
        if (_rb != null)
        {
            _rb.isKinematic = false;
            _rb.useGravity = true;
        }
    }

    private void SetCollidersEnabled(bool enabled)
    {
        // 防御：如果运行时模型结构变了，重新抓一次也行（GameJam够用）
        if (_allCols == null || _allCols.Length == 0)
            _allCols = GetComponentsInChildren<Collider>(true);

        foreach (var c in _allCols)
        {
            if (c != null) c.enabled = enabled;
        }
    }
}
