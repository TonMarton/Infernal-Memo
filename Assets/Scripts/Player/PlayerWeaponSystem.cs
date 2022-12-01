using System;
using UnityEngine;
using UnityEngine.Serialization;

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

    [Header("Impacts")]
    [SerializeField]
    public GameObject bloodImpactParticlePrefab;
    [SerializeField]
    public GameObject surfaceImpactParticlePrefab;
    [SerializeField]
    public float autoDestroyParticleTime = 3f;
    [SerializeField]
    public float particleSpawnOffset = 0.2f;

    [Header("Sound")]
    [SerializeField] private FMODUnity.EventReference staplerDrawSoundEvent;
    [SerializeField] private FMODUnity.EventReference staplerPutAwaySoundEvent;
    [SerializeField] private FMODUnity.EventReference pistolDrawSoundEvent;
    [SerializeField] private FMODUnity.EventReference pistolPutAwaySoundEvent;
    [SerializeField] private FMODUnity.EventReference shotgunDrawSoundEvent;
    [SerializeField] private FMODUnity.EventReference shotgunPutAwaySoundEvent;
    [Space]
    [SerializeField] public FMODUnity.EventReference brickBulletImpactSoundEvent;

    // Sounds
    private FMOD.Studio.EventInstance staplerDrawSoundInstance;
    private FMOD.Studio.EventInstance staplerPutAwaySoundInstance;
    private FMOD.Studio.EventInstance pistolDrawSoundInstance;
    private FMOD.Studio.EventInstance pistolPutAwaySoundInstance;
    private FMOD.Studio.EventInstance shotgunDrawSoundInstance;
    private FMOD.Studio.EventInstance shotgunPutAwaySoundInstance;



    [HideInInspector] public WeaponType currentWeaponType;
    private PlayerStaplerAttack staplerAttack;
    private Stapler stapler;
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
        stapler = GetComponentInChildren<Stapler>();
        pistol = GetComponentInChildren<Pistol>();
        shotgun = GetComponentInChildren<Shotgun>();
        hud = GetComponentInChildren<HUD>();
    }

    public void OnPlayerDie()
    {
        armsModel.SetActive(false);
        StopReload(currentWeaponType);
    }

    private void Start()
    {
        // hide crosshair by default
        hud.SetCrossHairVisible(false);

        // start with melee
        SwitchWeapon(WeaponType.Stapler, true);
    }

    // attack
    public void Attack()
    {
        if (playerStats.isDead) return;
        switch (currentWeaponType)
        {
            case WeaponType.Stapler:
                stapler.Shoot();
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

    private void StopReload(WeaponType weaponType)
    {
        switch (weaponType)
        {
            case WeaponType.Pistol:
                if (currentReloadCooldownTime > 0)
                {
                    pistol.StopReload();
                }
                break;
            case WeaponType.Shotgun:
                if (currentReloadCooldownTime > 0)
                {
                    shotgun.StopReload();
                }
                break;
        }
    }

    public void Reload()
    {
        if (playerStats.isDead) return;

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

    public void SwitchWeapon(WeaponType weaponType, bool forceSwitch=false)
    {
        if (playerStats.isDead) return;

        // don't switch if this is the weapon that's already selected
        if (currentWeaponType == weaponType && !forceSwitch)
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
                SoundUtils.PlaySound3D(ref staplerDrawSoundInstance, staplerDrawSoundEvent, gameObject);

                // TODO: Play weapon switch animation

                // Play Stapler Idle
                armsAnimator.Play(stapler.drawAnimationState, -1, 0);

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
                SoundUtils.PlaySound3D(ref pistolDrawSoundInstance, pistolDrawSoundEvent, gameObject);

                // TODO: Play weapon switch animation

                // Play Pistol Idle
                armsAnimator.Play(pistol.drawAnimationState, -1, 0);

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
                SoundUtils.PlaySound3D(ref shotgunDrawSoundInstance, shotgunDrawSoundEvent, gameObject);

                // TODO: Play weapon switch animation

                // Play Shotgun Idle
                armsAnimator.Play(shotgun.drawAnimationState, -1, 0);

                break;
            default:
                // debug log the value currentWeaponType
                Debug.Log("currentWeaponType = " + currentWeaponType);
                throw new ArgumentOutOfRangeException();
        }

        StopReload(lastWeaponType);

        currentCooldownTime = 0;
        currentReloadCooldownTime = 0;

        // show crosshair for shotgun and pistol only
        var visible = weaponType == WeaponType.Pistol || weaponType == WeaponType.Shotgun;
        hud.SetCrossHairVisible(visible);
    }

    private void Update()
    {
        if (playerStats.isDead) return;

        // update cooldown time
        currentCooldownTime -= Time.deltaTime;
        if (currentCooldownTime <= 0)
        {
            currentCooldownTime = 0;
        }

        if (currentReloadCooldownTime > 0)
        {
            currentReloadCooldownTime -= Time.deltaTime;
            if (currentReloadCooldownTime < 0)
            {
                if (currentWeaponType == WeaponType.Pistol
                    && playerStats.bullets > 0)
                {
                    playerStats.Reload(WeaponType.Pistol);
                }
                else if (currentWeaponType == WeaponType.Shotgun
                    && playerStats.shells > 0)
                {
                    playerStats.Reload(WeaponType.Shotgun);
                }
                currentReloadCooldownTime = 0;
            }
        }
        else if (currentCooldownTime == 0)
        {
            if (currentWeaponType == WeaponType.Pistol && playerStats.bulletsInClip == 0 && playerStats.bullets > 0)
            {
                Reload();
            }
            else if (currentWeaponType == WeaponType.Shotgun && playerStats.shellsInClip == 0 && playerStats.shells > 0)
            {
                Reload();
            }
        }
    }

    public void SwitchWeaponNext()
    {
        // switch to next weapon
        SwitchWeapon((WeaponType)(((int)currentWeaponType + 1) % System.Enum.GetValues(typeof(WeaponType)).Length));
    }

    public void SwitchWeaponPrevious()
    {
        // switch to previous weapon
        var index = currentWeaponType - 1;
        if (index < 0)
        {
            index = (WeaponType)(System.Enum.GetValues(typeof(WeaponType)).Length - 1);
        }
        SwitchWeapon((WeaponType)(((int)index) % System.Enum.GetValues(typeof(WeaponType)).Length));
    }
}