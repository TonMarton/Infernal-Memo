using System;
using UnityEngine;

// enum for melee and shotgun weapons
public enum WeaponType
{
    Stapler,
    Shotgun
}

[DisallowMultipleComponent]
public class PlayerWeaponSystem : MonoBehaviour
{

    [SerializeField] private HUD HUD;

    private WeaponType currentWeaponType;
    private PlayerStaplerAttack staplerAttack;
    
    private void Awake()
    {
        var HUD = GameObject.Find("Player");
        // set reference to stapler attack
        staplerAttack = GetComponent<PlayerStaplerAttack>();
        
        // start with melee
        currentWeaponType = WeaponType.Stapler;
    }
    
    // attack
    public void Attack()
    {
        switch (currentWeaponType)
        {
            case WeaponType.Stapler:
                // attack with melee
                staplerAttack.Attack();
                break;
            case WeaponType.Shotgun:
                // attack with shotgun
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public void SwitchWeapon(WeaponType weaponType)
    {
        // set current weapon
        currentWeaponType = weaponType;

        HUD.ToggleCrossHair(weaponType == WeaponType.Shotgun);
    }
    
    public void SwitchWeaponNext()
    {
        // switch to next weapon
        SwitchWeapon((WeaponType)(((int)currentWeaponType + 1) % System.Enum.GetValues(typeof(WeaponType)).Length));
    }
    
    public void SwitchWeaponPrevious()
    {
        // switch to previous weapon
        SwitchWeapon((WeaponType)(((int)currentWeaponType - 1) % System.Enum.GetValues(typeof(WeaponType)).Length));
    }
}