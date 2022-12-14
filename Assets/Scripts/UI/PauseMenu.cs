using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class PauseMenu : MonoBehaviour
{
    // Buttons
    [SerializeField] private Button continueButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button exitButton;

    [Header("Sound")]
    [SerializeField] private FMODUnity.EventReference hoverSoundEvent;
    [SerializeField] private FMODUnity.EventReference clickSoundEvent;
    [SerializeField] private FMODUnity.EventReference openSoundEvent;
    [SerializeField] private FMODUnity.EventReference closeSoundEvent;

    // Sounds
    private FMOD.Studio.EventInstance hoverSoundInstance;
    private FMOD.Studio.EventInstance clickSoundInstance;
    private FMOD.Studio.EventInstance openSoundInstance;
    private FMOD.Studio.EventInstance closeSoundInstance;

    private void Awake()
    {
        // click sounds
        continueButton.onClick.AddListener(PlayClickSound);
        mainMenuButton.onClick.AddListener(PlayClickSound);
        exitButton.onClick.AddListener(PlayClickSound);
        
        // hover sound for buttons
        EventTrigger trigger = continueButton.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((eventData) => { PlayHoverSound(); });
        trigger.triggers.Add(entry);
        
        trigger = mainMenuButton.GetComponent<EventTrigger>();
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((eventData) => { PlayHoverSound(); });
        trigger.triggers.Add(entry);
        
        trigger = exitButton.GetComponent<EventTrigger>();
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((eventData) => { PlayHoverSound(); });
        trigger.triggers.Add(entry);
        
        // hover sounds (using OnPointerEnter)
        continueButton.GetComponent<EventTrigger>().triggers.Add(new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter, callback = new EventTrigger.TriggerEvent() });
        mainMenuButton.GetComponent<EventTrigger>().triggers.Add(new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter, callback = new EventTrigger.TriggerEvent() });
        exitButton.GetComponent<EventTrigger>().triggers.Add(new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter, callback = new EventTrigger.TriggerEvent() });
        
        // continue button toggles the pause menu
        continueButton.onClick.AddListener(Toggle);

        // main menu button loads the main menu scene
        mainMenuButton.onClick.AddListener(OpenMainMenu);

        // exit button quits the game 
        exitButton.onClick.AddListener(Quit);
    }

    // toggle the pause menu
    public void Toggle()
    {
        // toggle the pause menu 
        gameObject.SetActive(!gameObject.activeSelf);

        // menu is open?
        if (gameObject.activeSelf)
        {
            // play menu open sound
            SoundUtils.PlaySound3D(ref openSoundInstance, openSoundEvent, gameObject);
        }
        // menu is closed?
        else
        {
            // play menu closed sound
            SoundUtils.PlaySound3D(ref closeSoundInstance, closeSoundEvent, gameObject);
        }

        // if pause menu is active, set time scale to 0
        if (gameObject.activeSelf)
        {
            Time.timeScale = 0;
        }
        // otherwise set time scale to 1
        else
        {
            Time.timeScale = 1;
        }
    }
    
    private void PlayClickSound()
    {
        SoundUtils.PlaySound3D(ref clickSoundInstance, clickSoundEvent, gameObject);
    }
    
    private void PlayHoverSound()
    {
        SoundUtils.PlaySound3D(ref hoverSoundInstance, hoverSoundEvent, gameObject);
    }

    private void OpenMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Title Screen");
    }

    private static void Quit()
    {
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
}