using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private PauseMenu pauseMenu;
    private PlayerWeaponSystem playerWeaponSystem;

    private void Awake()
    {
        // get references to player components that need to be controlled by input
        playerWeaponSystem = GetComponent<PlayerWeaponSystem>();
        
        // Set up the player input actions
        var inputActions = new InputActions();
        inputActions.Player.Enable();
        
        // toggle pause menu
        inputActions.Player.Pause.performed += _ctx => pauseMenu.Toggle();
        
        // switch weapon directly
        inputActions.Player.SelectWeaponMelee.performed += _ctx => playerWeaponSystem.SwitchWeapon(WeaponType.Stapler);
        inputActions.Player.SelectWeaponShotgun.performed += _ctx => playerWeaponSystem.SwitchWeapon(WeaponType.Shotgun);
        
        // switch weapon prev/next
        inputActions.Player.SelectPreviousWeapon.performed += _ctx => playerWeaponSystem.SwitchWeaponPrevious();
        inputActions.Player.SelectNextWeapon.performed += _ctx => playerWeaponSystem.SwitchWeaponNext();
        
        // attack
        inputActions.Player.Fire.performed += _ctx => playerWeaponSystem.Attack();
    }
}