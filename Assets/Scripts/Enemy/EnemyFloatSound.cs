using UnityEngine;


public class EnemyFloatSound : MonoBehaviour
{
    [Header("Sound")] [SerializeField] private FMODUnity.EventReference floatSoundEvent;

    // Sounds
    private FMOD.Studio.EventInstance floatSoundInstance;

    private bool soundStarted = false;

    private void Update()
    {
        if (!soundStarted)
        {
            soundStarted = true;
            PlayFloatSound();
        }
    }

    private void PlayFloatSound()
    {
        SoundUtils.PlaySound3D(ref floatSoundInstance, floatSoundEvent, gameObject);
    }
}