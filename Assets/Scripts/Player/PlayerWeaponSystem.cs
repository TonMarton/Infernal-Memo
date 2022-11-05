using System;
using UnityEngine;

// enum for melee and shotgun weapons
public enum WeaponType
{
    Stapler,
    Shotgun
}

public class PlayerWeaponSystem : MonoBehaviour
{
    private WeaponType currentWeaponType;
    private PlayerStaplerAttack staplerAttack;
    
    private void Awake()
    {
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
    }
    
    public void SwitchWeaponNext()
    {
        // switch to next weapon
        currentWeaponType = (WeaponType)(((int)currentWeaponType + 1) % System.Enum.GetValues(typeof(WeaponType)).Length);
    }
    
    public void SwitchWeaponPrevious()
    {
        // switch to previous weapon
        currentWeaponType = (WeaponType)(((int)currentWeaponType - 1) % System.Enum.GetValues(typeof(WeaponType)).Length);
    }
}