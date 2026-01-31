using UnityEngine;

public class GameState : MonoBehaviour
{
    public int loopIndex = 1;

    // 关键：你后面要记忆的状态都放这里
    public bool pathLocked = false;
    public string lastUsedMaskId = "";

    public void NextLoop()
    {
        loopIndex++;
        pathLocked = false;
        lastUsedMaskId = "";
    }

    public void ResetLoopSameIndex()
    {
        pathLocked = false;
        lastUsedMaskId = "";
    }
}
