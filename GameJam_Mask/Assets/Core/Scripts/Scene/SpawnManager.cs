using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private void Start()
    {
        string id = SceneLoadRequest.NextSpawnId;
        if (string.IsNullOrEmpty(id))
            id = SaveManager.I.Data.spawnId ?? "Default";

        var points = FindObjectsOfType<SpawnPoint>(true);
        SpawnPoint target = null;

        foreach (var p in points)
        {
            if (p.spawnId == id) { target = p; break; }
        }
        if (target == null && points.Length > 0) target = points[0];

        var player = GameObject.FindWithTag("Player");
        if (player != null && target != null)
        {
            player.transform.position = target.transform.position;
            player.transform.rotation = target.transform.rotation;
        }

        SceneLoadRequest.NextSpawnId = null;
    }
}
