namespace Scenes.Interaction {
    //所有物体的交互都在这里实现
    //注意：需要给物体添加3D 碰撞体（Box Collider/Sphere Collider/Capsule Collider），确保碰撞体完全包裹物体，取消勾选IsTrigger（除非有特殊触发需求）
    public interface IInteractable {
        public void OnInteract(int mouseButton);
    }
}