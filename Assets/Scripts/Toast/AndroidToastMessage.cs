using UnityEngine;

public class AndroidToastMessage : MonoBehaviour
{
    public void ShowToast(string message)
    {
        var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
        {
            var toastClass = new AndroidJavaClass("android.widget.Toast");
            var toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, "Hello, Android Toast!", 0);
            toastObject.Call("show");
        }));
    }
}