using UnityEngine;

public class SoundUtils : MonoBehaviour
{
    public static void PlaySound3D(FMOD.Studio.EventInstance instance, string eventName, GameObject gameObject)
    {
        instance = FMODUnity.RuntimeManager.CreateInstance($"event:/{eventName}");
        instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        instance.start();
        instance.release();
    }
}