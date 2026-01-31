using UnityEngine;

public class MaskSystem : MonoBehaviour
{
    public MaskDef currentMask; // 玩家手上“拿着”的面具

    private MaskDatabase _db;
    private SubtitleSystem _subs;
    private GameState _state;

    public void Init(MaskDatabase db, SubtitleSystem subs, GameState state)
    {
        _db = db;
        _subs = subs;
        _state = state;
    }

    public bool HasMask => currentMask != null;

    public void GiveMask(string maskId)
    {
        if (_db.TryGet(maskId, out var def))
        {
            currentMask = def;
            _subs.Show($"获得面具：{def.displayName}");
        }
        else
        {
            _subs.Show($"[错误] 未找到面具定义：{maskId}");
        }
    }

    public void ClearMask()
    {
        currentMask = null;
    }

    public void UseMaskOn(MaskTargetType target)
    {
        if (currentMask == null)
        {
            _subs.Show("你手上没有面具。");
            return;
        }

        if (_state.pathLocked)
        {
            _subs.Show("当前循环的路径已锁死（请进入下一轮再试）。");
            return;
        }

        // 这里就是“演出触发”和“逻辑分离”的入口点
        if (currentMask.targetType != target)
        {
            _subs.Show($"这个面具不能对 {target} 使用。");
            return;
        }

        _state.pathLocked = true;
        _state.lastUsedMaskId = currentMask.id;

        _subs.Show(currentMask.onUseSubtitle, 4f);

        // 用完清空（你也可以改成保留）
        currentMask = null;
    }
}
