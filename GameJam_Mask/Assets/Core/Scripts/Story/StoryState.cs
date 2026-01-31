using System;
using System.Collections.Generic;

[Serializable]
public class StoryState
{
    public int version = 1;

    // 循环次数（loop 1/2/3…）
    public int loopIndex = 1;

    // 当前场景&落点（存档恢复用）
    public string sceneName = "Menu";
    public string spawnId = "Default";

    // 剧情 flags（picked_ticket / wore_mask_xxx）
    public List<string> flags = new List<string>();

    // 剧情选择（wifeMask=Angry 等）
    public List<ChoiceKV> choices = new List<ChoiceKV>();

    [Serializable]
    public struct ChoiceKV
    {
        public string key;
        public string value;
    }
}
