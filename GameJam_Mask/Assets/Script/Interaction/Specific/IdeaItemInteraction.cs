using Ideas;
using Items;
using UnityEngine;

namespace Scenes.Interaction.Specific
{
    public class IdeaItemInteraction : MonoBehaviour, Scenes.Interaction.IInteractable
    {
        [SerializeField] public string IdeaId;

        private HoldableWorldObject _holdable;

        private void Awake()
        {
            _holdable = GetComponent<HoldableWorldObject>();
            if (_holdable == null)
                Debug.LogWarning("IdeaItemInteraction 建议同物体挂一个 HoldableWorldObject，用于悬停展示/回原位。", this);
        }

        public void OnInteract(int trigger)
        {
            // 只响应E（我们用 999 表示）
            // 你也可以不判断 trigger，反正现在 InteractDetector 只会发E
            // if (trigger != 999) return;

            // 如果当前正在拿着这个物体：第二次E确认
            if (_holdable != null && _holdable.IsHeld)
            {
                ConfirmSecondPress();
                return;
            }

            // 第一次E：拿起悬停 + 字幕
            FirstPressPickUp();
        }

        private void FirstPressPickUp()
        {
            if (_holdable != null)
            {
                _holdable.BeginHold(Camera.main.transform);
                ToastManager.DisplayToast($"{_holdable.title}\n{_holdable.description}");
            }
            else
            {
                // 没挂 HoldableWorldObject 也别崩：至少给出提示
                ToastManager.DisplayToast($"拾取：{this.IdeaId}");
            }
        }

        private void ConfirmSecondPress()
        {
            // 取到念头定义
            IdeaManager.Idea idea = IdeaManager.Ideas[this.IdeaId];
            if (idea == null)
            {
                Debug.LogError($"Unknown id {this.IdeaId} for idea", this);
                // 出错也回原位，避免卡死
                _holdable?.EndHold_ReturnToOrigin();
                return;
            }

            // 生成 wrapper（你们已有）
            Item wrapper = IdeaWrapper.ByIdea[idea];

            // 按文案：决定“入栏位 or 回原位”
            if (_holdable != null && _holdable.confirmStoreIntoInventory)
            {
                // 入栏位（不允许重复）
                if (PlayDataManager.Inventory.TryInsertItem(PlayDataManager.ViewingItem = wrapper))
                {
                    // 成功：隐藏世界物体 + 显示字幕
                    _holdable.EndHold_StoreAndHide();
                    ToastManager.DisplayToast(idea.Message);
                }
                else
                {
                    // 失败：回原位，避免物体丢失
                    ToastManager.DisplayToast("栏位已有该物品（或不可重复），已放回原位。");
                    _holdable.EndHold_ReturnToOrigin();
                }
            }
            else
            {
                // 配置为回原位（不可入栏位/无需合成）
                ToastManager.DisplayToast("查看完毕，已放回原位。");
                _holdable?.EndHold_ReturnToOrigin();
            }
        }
    }
}
