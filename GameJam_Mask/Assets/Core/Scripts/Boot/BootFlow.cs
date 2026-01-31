using UnityEngine;
using UnityEngine.SceneManagement;

public class BootFlow : MonoBehaviour
{
    [SerializeField] private string menuSceneName = "Menu";

    private void Start()
    {
        Debug.Log($"[BootFlow] Start in scene: {SceneManager.GetActiveScene().name}");

        // 防止 SceneLoader 还没准备好（顺序问题）
        if (SceneLoader.I == null)
        {
            Debug.LogError("[BootFlow] SceneLoader.I is null. Make sure SceneLoader exists in Boot/Systems.");
            return;
        }

        Debug.Log($"[BootFlow] Loading menu: {menuSceneName}");
        SceneLoader.I.Load(menuSceneName, "Default");
    }
}
