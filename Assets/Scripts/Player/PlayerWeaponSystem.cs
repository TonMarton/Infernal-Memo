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
    private Shotgun shotgun;
    
    private void Awake()
    {
        // set reference to stapler attack
        staplerAttack = GetComponent<PlayerStaplerAttack>();
        
        // set reference to shotgun 
        shotgun = GetComponentInChildren<Shotgun>();
        
        // start with melee
        currentWeaponType = WeaponType.Stapler;
    }
    
    // attack
    public void Attack()
    {
        switch (currentWeaponType)
        {
            case WeaponType.Stapler:
                staplerAttack.Attack();
                break;
            case WeaponType.Shotgun:
                shotgun.Shoot();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public void SwitchWeapon(WeaponType weaponType)
    {
        // log the weapon we switched to
        Debug.Log("Switched to " + weaponType);
        
        // set current weapon
        currentWeaponType = weaponType;

        // show crosshair for shotgun
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