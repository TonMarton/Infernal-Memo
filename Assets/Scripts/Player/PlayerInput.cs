using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private PauseMenu pauseMenu;

    private void Awake()
    {
        // Set up the player input actions
        var inputActions = new InputActions();
        inputActions.Player.Enable();
        inputActions.Player.Pause.performed += _ctx => pauseMenu.Toggle();
    }
}