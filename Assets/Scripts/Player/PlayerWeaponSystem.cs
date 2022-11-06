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
    private HUD hud;

    private void Awake()
    {
        staplerAttack = GetComponent<PlayerStaplerAttack>();
        shotgun = GetComponentInChildren<Shotgun>();
        hud = GetComponentInChildren<HUD>();

        // start with melee
        currentWeaponType = WeaponType.Stapler;
    }

    private void Start()
    {
        // hide crosshair by default
        hud.SetCrossHairVisible(false);
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

        // update weapon visibilities
        switch (currentWeaponType)
        {
            case WeaponType.Stapler:
                // hide shotgun
                shotgun.SetVisible(false);
                // TODO: show stapler
                // TODO: play stapler show sound
                break;
            case WeaponType.Shotgun:
                // show shotgun
                shotgun.SetVisible(true);
                // TODO: hide stapler
                // TODO: play shotgun show sound
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        // show crosshair for shotgun only
        var visible = weaponType == WeaponType.Shotgun;
        hud.SetCrossHairVisible(visible);
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