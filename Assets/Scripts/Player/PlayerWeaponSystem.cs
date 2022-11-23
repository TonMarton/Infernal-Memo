﻿using System;
using UnityEngine;
using UnityEngine.Serialization;

// enum for melee and shotgun weapons
public enum WeaponType
{
    None,
    Stapler,
    Pistol,
    Shotgun,
}

[DisallowMultipleComponent]
public class PlayerWeaponSystem : MonoBehaviour
{
    [SerializeField] private GameObject armsModel; // the base transform for the arms and all the weapons
    [SerializeField] private GameObject staplerModel; // stapler only
    [SerializeField] private GameObject deagleModel; // deagle only
    [SerializeField] private GameObject shotgunModel; // shotgun only

    public Animator armsAnimator;
    // TODO: customize the layer mask so it doesn't hit things like trigger volumes
    [FormerlySerializedAs("layerMask")]
    [SerializeField]
    public LayerMask collisionLayerMask = Physics.DefaultRaycastLayers;

    [SerializeField]
    public Transform fpsCam;

    [Header("Bullet Hole")]
    [SerializeField]
    public GameObject bulletHolePrefab;

    [SerializeField]
    public float autoDestroyBulletHoleTime = 10f;
    [Header("Sound")]
    [SerializeField] private FMODUnity.EventReference staplerDrawSoundEvent;
    [SerializeField] private FMODUnity.EventReference staplerPutAwaySoundEvent;
    [SerializeField] private FMODUnity.EventReference pistolDrawSoundEvent;
    [SerializeField] private FMODUnity.EventReference pistolPutAwaySoundEvent;
    [SerializeField] private FMODUnity.EventReference shotgunDrawSoundEvent;
    [SerializeField] private FMODUnity.EventReference shotgunPutAwaySoundEvent;

    // Sounds
    private FMOD.Studio.EventInstance staplerDrawSoundInstance;
    private FMOD.Studio.EventInstance staplerPutAwaySoundInstance;
    private FMOD.Studio.EventInstance pistolDrawSoundInstance;
    private FMOD.Studio.EventInstance pistolPutAwaySoundInstance;
    private FMOD.Studio.EventInstance shotgunDrawSoundInstance;
    private FMOD.Studio.EventInstance shotgunPutAwaySoundInstance;

    [HideInInspector] public WeaponType currentWeaponType;
    private PlayerStaplerAttack staplerAttack;
    private Pistol pistol;
    private Shotgun shotgun;
    private HUD hud;

    

    public PlayerStats playerStats { get; private set; }

    public float currentCooldownTime { get; set; }
    public float lastFireTime { get; set; }
    public float currentReloadCooldownTime { get; set; }

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        staplerAttack = GetComponent<PlayerStaplerAttack>();
        pistol = GetComponentInChildren<Pistol>();
        shotgun = GetComponentInChildren<Shotgun>();
        hud = GetComponentInChildren<HUD>();
    }

    private void Start()
    {
        // hide crosshair by default
        hud.SetCrossHairVisible(false);

        // hide arms by default
        armsModel.SetActive(false);

        // start with melee
        SwitchWeapon(WeaponType.Stapler);
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
                if (playerStats.bulletsInClip > 0)
                {
                    pistol.Shoot();
                }
                break;
            case WeaponType.Shotgun:
                if (playerStats.shellsInClip > 0)
                {
                    shotgun.Shoot();
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Reload()
    {
        switch (currentWeaponType)
        {
            case WeaponType.Stapler:
                // no stapler reload
                break;
            case WeaponType.Pistol:
                if (playerStats.bullets > 0 && playerStats.bulletsInClip != playerStats.maxBulletsInClip)
                {
                    pistol.Reload();
                }
                break;
            case WeaponType.Shotgun:
                if (playerStats.shells > 0 && playerStats.shellsInClip != playerStats.maxShellsInClip)
                {
                    shotgun.Reload();
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void SwitchWeapon(WeaponType weaponType)
    {
        // log the weapon we switched to
        Debug.Log("Switched to " + weaponType);

        // don't switch if this is the weapon that's already selected
        if (currentWeaponType == weaponType)
        {
            return;
        }

        // remember the last weapon type
        var lastWeaponType = currentWeaponType;

        // set current weapon
        currentWeaponType = weaponType;

        // update weapon visibilities
        switch (currentWeaponType)
        {
            case WeaponType.Stapler:
                // show arms
                armsModel.SetActive(true);


                // hide shotgun
                shotgun.SetVisible(false);
                // hide pistol
                deagleModel.gameObject.SetActive(false);
                // show stapler
                staplerModel.gameObject.SetActive(true);

                // play draw stapler sound
                SoundUtils.PlaySound3D(staplerDrawSoundInstance, staplerDrawSoundEvent, gameObject);

                // TODO: Play weapon switch animation

                // Play Stapler Idle
                armsAnimator.Play("StaplerIdlePlaceholder", -1, 0);

                break;

            case WeaponType.Pistol:
                // show arms
                armsModel.SetActive(true);
                // hide stapler
                staplerModel.gameObject.SetActive(false);
                // show pistol
                deagleModel.gameObject.SetActive(true);
                // hide shotgun
                shotgun.SetVisible(false);
                // play draw pistol sound
                SoundUtils.PlaySound3D(pistolDrawSoundInstance, pistolDrawSoundEvent, gameObject);
                // Play Pistol Idle
                armsAnimator.Play("Pistol Idle", -1, 0);

                // TODO: Play weapon switch animation
                break;

            case WeaponType.Shotgun:
                // show arms
                armsModel.SetActive(true);
                // show shotgun
                shotgun.SetVisible(true);
                // hide stapler
                staplerModel.gameObject.SetActive(false);
                // hide pistol
                deagleModel.gameObject.SetActive(false);
                // play draw shotgun sound
                SoundUtils.PlaySound3D(shotgunDrawSoundInstance, shotgunDrawSoundEvent, gameObject);
                // Play Shotgun Idle
                armsAnimator.Play("Shotgun Idle", -1, 0);

                // TODO: Play weapon switch animation
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        currentCooldownTime = 0;
        currentReloadCooldownTime = 0;

        // show crosshair for shotgun and pistol only
        var visible = weaponType == WeaponType.Pistol || weaponType == WeaponType.Shotgun;
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