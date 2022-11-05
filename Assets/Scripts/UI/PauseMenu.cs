using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    // hold a reference to continue, main menu, and exit buttons
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject mainMenuButton;
    [SerializeField] private GameObject exitButton;

    private void Awake()
    {
        // bind escape to open pause menu
        // InputManager.Instance.BindAction("Pause", OpenPauseMenu, InputManager.INPUT_STATE.PRESSED);
    }

    public void Activate()
    {
        // set the buttons to be active
        continueButton.SetActive(true);
        mainMenuButton.SetActive(true);
        exitButton.SetActive(true);
    }

    public void Close()
    {
        // set the buttons to be inactive
        continueButton.SetActive(false);
        mainMenuButton.SetActive(false);
        exitButton.SetActive(false);
    }
}