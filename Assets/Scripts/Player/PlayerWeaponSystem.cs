using System;
using UnityEngine;

// enum for melee and shotgun weapons
public enum WeaponType
{
    Stapler,
    Pistol,
    Shotgun,
}

[DisallowMultipleComponent]
public class PlayerWeaponSystem : MonoBehaviour
{
    [Header("Sound")] 
    [SerializeField] private FMODUnity.EventReference shotgunDrawSoundEvent;
    [SerializeField] private FMODUnity.EventReference staplerDrawSoundEvent;
    [SerializeField] private FMODUnity.EventReference shotgunPutAwaySoundEvent;
    [SerializeField] private FMODUnity.EventReference staplerPutAwaySoundEvent;

    // Sounds
    private FMOD.Studio.EventInstance shotgunDrawSoundInstance;
    private FMOD.Studio.EventInstance staplerDrawSoundInstance;
    private FMOD.Studio.EventInstance shotgunPutAwaySoundInstance;
    private FMOD.Studio.EventInstance staplerPutAwaySoundInstance;

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
            case WeaponType.Pistol:
                // TODO: pistol shoot
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
        
        // remember the last weapon type
        var lastWeaponType = currentWeaponType;

        // set current weapon
        currentWeaponType = weaponType;

        // update weapon visibilities
        switch (currentWeaponType)
        {
            case WeaponType.Stapler:
                // hide shotgun
                shotgun.SetVisible(false);
                // TODO: hide pistol
                // TODO: show stapler
                // TODO: play stapler show sound
                // play draw stapler sound
                SoundUtils.PlaySound3D(staplerDrawSoundInstance, staplerDrawSoundEvent, gameObject);
                break;
            case WeaponType.Pistol:
                // TODO: hide stapler
                // TODO: hide shotgun
                // TODO: draw pistol sound
            case WeaponType.Shotgun:
                // show shotgun
                shotgun.SetVisible(true);
                // TODO: hide stapler
                // TODO: hide pistol
                // TODO: play shotgun show sound
                // play draw shotgun sound
                SoundUtils.PlaySound3D(shotgunDrawSoundInstance, shotgunDrawSoundEvent, gameObject);
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