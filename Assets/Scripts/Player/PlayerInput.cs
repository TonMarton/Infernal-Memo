using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class PlayerInput : MonoBehaviour
{
    [SerializeField] private PauseMenu pauseMenu;
    private PlayerWeaponSystem playerWeaponSystem;

    InputActions inputActions;

    private void Awake()
    {
        // get references to player components that need to be controlled by input
        playerWeaponSystem = GetComponent<PlayerWeaponSystem>();
        
        // Set up the player input actions
        inputActions = new InputActions();
        inputActions.Player.Enable();
        
        // toggle pause menu
        inputActions.Player.Pause.performed += _ctx => pauseMenu.Toggle();
        
        // switch weapon directly
        inputActions.Player.SelectWeaponMelee.performed += _ctx => playerWeaponSystem.SwitchWeapon(WeaponType.Stapler);
        inputActions.Player.SelectWeaponDeagle.performed += _ctx => playerWeaponSystem.SwitchWeapon(WeaponType.Pistol);
        inputActions.Player.SelectWeaponShotgun.performed += _ctx => playerWeaponSystem.SwitchWeapon(WeaponType.Shotgun);

        // reload weapon
        inputActions.Player.Reload.performed += _ctx => playerWeaponSystem.Reload();
        
        // switch weapon prev/next
        inputActions.Player.SelectPreviousWeapon.performed += _ctx => playerWeaponSystem.SwitchWeaponPrevious();
        inputActions.Player.SelectNextWeapon.performed += _ctx => playerWeaponSystem.SwitchWeaponNext();
        
    }

    private void Update()
    {

        // attack
        if (inputActions.Player.Fire.IsPressed())
        {
            playerWeaponSystem.Attack();
        }
    }
}