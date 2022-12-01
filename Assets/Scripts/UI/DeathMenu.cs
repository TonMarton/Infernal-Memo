using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class DeathMenu : MonoBehaviour
{
    [SerializeField] private Button restartButton;
    
    [Header("Sound")]
    [SerializeField] private FMODUnity.EventReference deathSoundEvent;
    
    // Sounds
    private FMOD.Studio.EventInstance deathEventInstance;

    private void Awake()
    {
        // continue button toggles the pause menu
        restartButton.onClick.AddListener(Restart);
    }

    public void Show()
    {
        // Show mouse cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // enable the death menu
        gameObject.SetActive(true);
        gameObject.transform.parent.gameObject.GetComponentInChildren<HUD>().gameObject.SetActive(false);
        
        // death sound
        SoundUtils.PlaySound3D(ref deathEventInstance, deathSoundEvent, gameObject);
    }

    // restart the game
    private void Restart()
    {
        // Hide mouse cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // disable the death menu
        gameObject.SetActive(false);

        // load the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}