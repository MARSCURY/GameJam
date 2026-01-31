using UnityEngine;

public static class ToastManager
{
    public static void DisplayToast(string message)
    {
        Debug.Log($"[Toast] {message}");
    }
}
