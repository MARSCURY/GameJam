using UnityEngine;

public class CraftSystem : MonoBehaviour
{
    private ThoughtInventory _inv;
    private RecipeBook _book;
    private MaskSystem _mask;
    private SubtitleSystem _subs;

    public void Init(ThoughtInventory inv, RecipeBook book, MaskSystem mask, SubtitleSystem subs)
    {
        _inv = inv;
        _book = book;
        _mask = mask;
        _subs = subs;
    }

    public void TryCraft(Thought a, Thought b)
    {
        if (a == null || b == null)
        {
            _subs.Show("请选择两个念头进行合成。");
            return;
        }
        if (a == b)
        {
            _subs.Show("同一个念头不能合成。");
            return;
        }

        if (_book.TryCraft(a.id, b.id, out var maskId))
        {
            _inv.Remove(a);
            _inv.Remove(b);
            _mask.GiveMask(maskId);
        }
        else
        {
            _subs.Show($"合成失败：{a.displayName} + {b.displayName}");
        }
    }
}
