using Items;
using JetBrains.Annotations;
using Scenes;

public static class PlayDataManager {
    public static SceneStateManager SceneStateManager { get; } = new();
    public static Inventory Inventory { get; } = new();
    //正在检视的物品，通过E来拿起&放下
    [CanBeNull] public static Item ViewingItem { get; set; }
}