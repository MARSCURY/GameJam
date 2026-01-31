using UnityEngine;

public class PlayerBootstrapper : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    private void Awake()
    {
        if (GameObject.FindWithTag("Player") != null) return;

        if (playerPrefab == null)
        {
            Debug.LogError("[PlayerBootstrapper] playerPrefab not assigned.");
            return;
        }

        var player = Instantiate(playerPrefab);
        player.tag = "Player";
        DontDestroyOnLoad(player);
    }
}
