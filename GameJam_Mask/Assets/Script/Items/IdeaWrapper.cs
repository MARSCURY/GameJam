using System.Collections.Generic;
using Ideas;

namespace Items {
    public class IdeaWrapper : Item {
        public static Dictionary<IdeaManager.Idea, Item> ByIdea { get; } = new();

        public IdeaManager.Idea Idea { get; }

        public IdeaWrapper(IdeaManager.Idea idea) {
            this.Idea = idea;
            ByIdea[idea] = this;
        }
    }
}