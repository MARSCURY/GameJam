using UnityEngine;

namespace Scenes.Interaction.Specific {
    public class PuppetInteraction : MonoBehaviour, IInteractable {
        public void OnInteract(int mouseButton) {
            PlayDataManager.Inventory.GetHoldingItem()?.OnInteract(this);
        }
    }
}