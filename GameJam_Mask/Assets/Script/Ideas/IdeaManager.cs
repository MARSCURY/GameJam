using System.Collections.Generic;
using Items;

namespace Ideas
{
    // 念头
    public static class IdeaManager
    {
        // 保留原有 API：外部脚本在用 IdeaManager.Ideas["id"]
        public static Dictionary<string, Idea> Ideas { get; } = new Dictionary<string, Idea>();

        // 建议加个安全注册方法（可选，但很有用）
        public static void Register(string id, string title, string message, string texture = "")
        {
            Ideas[id] = new Idea(title, message, texture);
        }

        // 不用 record，改成 Unity 兼容的 class，并把字段补齐
        public class Idea
        {
            public string Title { get; }
            public string Message { get; }
            public string Texture { get; }

            public Idea(string title, string message, string texture = "")
            {
                Title = title;
                Message = message;
                Texture = texture;

                // 保留你们原来的“创建 wrapper 并写入 ByIdea”逻辑
                _ = new IdeaWrapper(this);
            }
        }
    }
}
