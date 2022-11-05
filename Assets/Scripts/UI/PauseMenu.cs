using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button exitButton;

    private void Awake()
    {
        // continue button toggles the pause menu
        continueButton.onClick.AddListener(Toggle);

        // exit button quits the game 
        exitButton.onClick.AddListener(Quit);
    }

    // toggle the pause menu
    public void Toggle()
    {
        // toggle the pause menu 
        gameObject.SetActive(!gameObject.activeSelf);

        // if pause menu is active, show the mouse cursor
        if (gameObject.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        // otherwise hide the mouse cursor
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
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