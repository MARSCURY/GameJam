using UnityEngine;

public static class ItemViewingManager {
    public static void UpdateInteraction() {
        if (Input.GetKeyDown(KeyCode.E))
            PlayDataManager.ViewingItem = PlayDataManager.ViewingItem != null ? null : PlayDataManager.Inventory.GetHoldingItem();
    }
}