using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Instructions : MonoBehaviour
{
    // Buttons
    [SerializeField] private Button playButton;
    
    [Header("Sound")]
    [SerializeField] private FMODUnity.EventReference hoverSoundEvent;
    [SerializeField] private FMODUnity.EventReference startGameSoundEvent;
    [SerializeField] private FMODUnity.EventReference clickSoundEvent;
    
    // Sounds
    private FMOD.Studio.EventInstance hoverSoundInstance;
    private FMOD.Studio.EventInstance startGameSoundInstance;
    private FMOD.Studio.EventInstance clickSoundInstance;
    
    // awake
    private void Awake()
    {
        // click sounds
        playButton.onClick.AddListener(PlayClickSound);
        
        // hover sound for buttons
        EventTrigger trigger = playButton.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        entry.callback.AddListener((eventData) => { PlayHoverSound(); });
        trigger.triggers.Add(entry);
    }
    
    private void PlayClickSound()
    {
        SoundUtils.PlaySound3D(ref clickSoundInstance, clickSoundEvent, gameObject);
    }

    
    private void PlayHoverSound()
    {
        Debug.Log("playing hover sound");
        SoundUtils.PlaySound3D(ref hoverSoundInstance, hoverSoundEvent, gameObject);
    }

    public void PlayGame()
    {
        
        SoundUtils.PlaySound3D(ref startGameSoundInstance, startGameSoundEvent, gameObject);
        
        SceneManager.LoadScene(2);
    }
}