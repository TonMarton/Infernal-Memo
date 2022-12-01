
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class WinMenu : MonoBehaviour
{
    [SerializeField] private Button restartButton;

    private void Awake()
    {
        // continue button toggles the pause menu
        restartButton.onClick.AddListener(Restart);
    }

    public void Show()
    {
        // Show mouse cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;

        // enable the win menu
        gameObject.SetActive(true);
        gameObject.transform.parent.gameObject.GetComponentInChildren<HUD>().gameObject.SetActive(false);
    }

    // restart the game
    private void Restart()
    {
        // get player stats component
        PlayerStats playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        playerStats.didWin = false;
        // Hide mouse cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // disable the death menu
        gameObject.SetActive(false);

        // load the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}