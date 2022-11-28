using FMOD.Studio;
using UnityEngine;

public class SoundUtils : MonoBehaviour
{
    public static void PlaySound3D(ref FMOD.Studio.EventInstance instance, string eventName, GameObject gameObject)
    {
        instance = CreateInstance(eventName);
        PlaySound3DCommon(instance, gameObject);
    }
    
    public static void PlaySound3D(ref FMOD.Studio.EventInstance instance, FMODUnity.EventReference eventRef, GameObject gameObject)
    {
        instance = CreateInstance(eventRef);
        PlaySound3DCommon(instance, gameObject);
    }

    public static void StopSound3D(FMOD.Studio.EventInstance instance)
    {
        instance.stop(STOP_MODE.IMMEDIATE);
    }

    public static void PlaySound3DParameter(FMOD.Studio.EventInstance instance, FMODUnity.EventReference eventRef, GameObject gameObject, string parameterName, float parameterValue)
    {
        instance = CreateInstance(eventRef);
        PlaySound3DCommonParameter(instance, gameObject, parameterName, parameterValue);
    }

    private static void PlaySound3DCommon(FMOD.Studio.EventInstance instance, GameObject gameObject)
    {
        instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        instance.start();
        instance.release();
    }

    private static void PlaySound3DCommonParameter(FMOD.Studio.EventInstance instance, GameObject gameObject, string parameterName, float parameterValue)
    {
        instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        instance.setParameterByName(parameterName, parameterValue);
        instance.start();
        instance.release();
    }

    private static FMOD.Studio.EventInstance CreateInstance(FMODUnity.EventReference eventRef)
    {
        return FMODUnity.RuntimeManager.CreateInstance(eventRef);
    }
    
    private static FMOD.Studio.EventInstance CreateInstance(string eventName)
    {
        return FMODUnity.RuntimeManager.CreateInstance($"event:/{eventName}");
    }
}