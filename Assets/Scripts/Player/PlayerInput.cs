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
        //inputActions.Player.Pause.performed += _ctx => ;
    }

    private void Update()
    {
        if (inputActions.Player.Pause.WasPressedThisFrame())
        {
            pauseMenu.Toggle();
        }

        if (Time.timeScale == 1)
        {
            // attack
            if (inputActions.Player.Fire.IsPressed())
            {
                playerWeaponSystem.Attack();
            }

            //reload
            if (inputActions.Player.Reload.WasPressedThisFrame())
            {
                playerWeaponSystem.Reload();
            }

            //switch to last weapon
            if (inputActions.Player.SelectPreviousWeapon.WasPressedThisFrame())
            {
                playerWeaponSystem.SwitchWeaponPrevious();
            }
            //switch to next weapon
            if (inputActions.Player.SelectNextWeapon.WasPressedThisFrame())
            {
                playerWeaponSystem.SwitchWeaponNext();
            }

            //switch to first weapon
            if (inputActions.Player.SelectWeaponMelee.WasPressedThisFrame())
            {
                playerWeaponSystem.SwitchWeapon(WeaponType.Stapler);
            }
            //switch to second weapon
            if (inputActions.Player.SelectWeaponDeagle.WasPressedThisFrame())
            {
                playerWeaponSystem.SwitchWeapon(WeaponType.Pistol);
            }
            //switch to third weapon
            if (inputActions.Player.SelectWeaponShotgun.WasPressedThisFrame())
            {
                playerWeaponSystem.SwitchWeapon(WeaponType.Shotgun);
            }
        }
    }
}