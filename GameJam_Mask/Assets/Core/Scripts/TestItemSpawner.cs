using UnityEngine;

public class TestItemSpawner : MonoBehaviour
{
    private ThoughtInventory _inv;
    private SubtitleSystem _subs;

    public void Init(ThoughtInventory inv, SubtitleSystem subs)
    {
        _inv = inv;
        _subs = subs;
    }

    private void Update()
    {
        // 你可以用数字键快速模拟“点击物品获得念头”
        if (Input.GetKeyDown(KeyCode.Alpha1)) Give("ticket_hidden", "藏起的车票");
        if (Input.GetKeyDown(KeyCode.Alpha2)) Give("old_bankbook", "旧存折");
        if (Input.GetKeyDown(KeyCode.Alpha3)) Give("south_job", "南下的机会");
        if (Input.GetKeyDown(KeyCode.Alpha4)) Give("daughter_score", "女儿的满分");
    }

    private void Give(string id, string name)
    {
        _inv.Add(new Thought(id, name));
        _subs.Show($"获得念头：{name}");
    }
}
