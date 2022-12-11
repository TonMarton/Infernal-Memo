using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    // Buttons
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    
    [Header("Sound")]
    [SerializeField] private FMODUnity.EventReference hoverSoundEvent;
    [SerializeField] private FMODUnity.EventReference startGameSoundEvent;
    [SerializeField] private FMODUnity.EventReference clickSoundEvent;
    
    // Sounds
    private FMOD.Studio.EventInstance hoverSoundInstance;
    private FMOD.Studio.EventInstance startGameSoundInstance;
    private FMOD.Studio.EventInstance clickSoundInstance;
    
    public void PlayGame() {
        Debug.Log("load instructions");
        SceneManager.LoadScene("Instructions Screen");
    }
    
    private void Awake() {
        // click sounds
        playButton.onClick.AddListener(PlayClickSound);
        quitButton.onClick.AddListener(PlayClickSound);

        
        // hover sound for buttons
        EventTrigger trigger = playButton.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        entry.callback.AddListener((eventData) => { PlayHoverSound(); });
        trigger.triggers.Add(entry);
        
        trigger = quitButton.GetComponent<EventTrigger>();
        entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        entry.callback.AddListener((eventData) => { PlayHoverSound(); });
        trigger.triggers.Add(entry);
        
        // hover sounds (using OnPointerEnter)
        playButton.GetComponent<EventTrigger>().triggers.Add(new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter, callback = new EventTrigger.TriggerEvent() });
        quitButton.GetComponent<EventTrigger>().triggers.Add(new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter, callback = new EventTrigger.TriggerEvent() });
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
    public void QuitGame() {
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
}
