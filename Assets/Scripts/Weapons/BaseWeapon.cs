using UnityEngine;
using UnityEngine.Serialization;

public abstract class BaseWeapon : MonoBehaviour
{
    [SerializeField]
    public GameObject model;

    [Min(1)][SerializeField] protected int bulletCount = 20;
    [Min(0)][SerializeField] protected int shellsShootCost = 2;
    [Min(0)][SerializeField] protected float cooldownTime = 1.4f;
    [Min(0)]
    [SerializeField] protected float reloadCooldownTime = 1f;
    [Min(1)][SerializeField] protected float damagePerBullet = 2f;
    private WeaponSharedComponent weaponShared;
    private PlayerWeaponSystem weaponSystem;

    [Header("Muzzle Flash")]
    [SerializeField]
    private GameObject muzzleFlash;

    [SerializeField] private float hideMuzzleFlashAfterTime = 0.1f;

    [Header("Bullet Spread")]
    [SerializeField]
    [Min(0f)]
    [Tooltip("Degrees for spread on y-axis (horizontal)")]
    private float maxSpreadDegreesY = 5f;

    [SerializeField]
    [Min(0f)]
    [Tooltip("Degrees for spread on x-axis (vertical)")]
    private float maxSpreadDegreesX = 5f;

    [FormerlySerializedAs("shootSound")]
    [Header("Sound")]
    [SerializeField]
    private FMODUnity.EventReference shootSoundEvent;
    [FormerlySerializedAs("reloadSound")]
    [FormerlySerializedAs("reloadShootSound")]
    [SerializeField]
    private FMODUnity.EventReference reloadSoundEvent;

    // Sounds
    private FMOD.Studio.EventInstance shootSoundInstance;
    private FMOD.Studio.EventInstance reloadSoundInstance;

    public string shootAnimationState;
    public string reloadAnimationState;

    private void Awake()
    {
        weaponSystem = GetComponent<PlayerWeaponSystem>();
        weaponShared = GetComponent<WeaponSharedComponent>();

        // hide shotgun and muzzle flash to start
        SetVisible(false);
        SetMuzzleFlashVisible(false);
    }

    private void ShootBullet()
    {
        // get the camera's forward direction
        Vector3 fpsCamForward = weaponShared.fpsCam.forward;

        // calculate random spread
        float degreesX = Random.Range(-maxSpreadDegreesX, maxSpreadDegreesX);
        float degreesY = Random.Range(-maxSpreadDegreesY, maxSpreadDegreesY);

        // calculate trajectory
        Quaternion rotationX = Quaternion.AngleAxis(degreesX, weaponShared.fpsCam.right);
        Quaternion rotationY = Quaternion.AngleAxis(degreesY, weaponShared.fpsCam.up);
        Vector3 bulletTrajectory = rotationX * rotationY * fpsCamForward;

        // Raycast for hit
        const float range = 10000f; // max distance
        if (!Physics.Raycast(weaponShared.fpsCam.transform.position, bulletTrajectory, out var hit, range, weaponShared.collisionLayerMask))
        {
            // didn't hit so nothing to do
            return;
        }

        // Not an enemy?
        if (hit.collider.gameObject.layer != LayerMask.NameToLayer(Enemy.EnemyLayer))
        {
            // Create a bullet hole
            CreateBulletHoleDecal(hit.point, hit.normal);
        }

        // find the game object that was hit
        // and see if it was an enemy
        var hitObject = hit.collider.gameObject;

        // log the tag of the object that was hit
        if (!hitObject.CompareTag(Enemy.EnemyTag))
        {
            // wasn't an enemy so nothing to do
            return;
        }

        // damage the enemy
        var enemy = hitObject.GetComponentInParent<Enemy>();
        var enemyStats = enemy.GetComponent<EnemyStats>();
        enemyStats.TakeDamage(damagePerBullet, knockback: null);
    }

    public void SetVisible(bool visible)
    {
        model.SetActive(visible);
    }

    private void SetMuzzleFlashVisible(bool visible)
    {
        muzzleFlash.SetActive(visible);
    }

    private void Update()
    {
        // update cooldown time
        weaponShared.currentCooldownTime -= Time.deltaTime;

        // hide muzzle flash after a delay
        if (muzzleFlash.activeSelf && Time.time > weaponShared.lastFireTime + hideMuzzleFlashAfterTime)
        {
            SetMuzzleFlashVisible(false);
        }

        if (weaponShared.currentReloadCooldownTime > 0)
        {
            weaponShared.currentReloadCooldownTime -= Time.deltaTime;
            if (weaponShared.currentReloadCooldownTime < 0)
            {
                if (weaponSystem.currentWeaponType == WeaponType.Pistol
                    && weaponShared.playerStats.bullets > 0)
                {
                    weaponShared.playerStats.Reload(WeaponType.Pistol);
                }
                else if (weaponSystem.currentWeaponType == WeaponType.Shotgun
                    && weaponShared.playerStats.shells > 0)
                {
                    weaponShared.playerStats.Reload(WeaponType.Shotgun);
                }
                weaponShared.currentReloadCooldownTime = 0;
            }
        }
    }

    private void CreateBulletHoleDecal(Vector3 point, Vector3 normal)
    {
        // spawn a bullet hole decal
        GameObject bulletHole = Instantiate(weaponShared.bulletHolePrefab, point + normal * Random.Range(0.001f, 0.002f),
            Quaternion.LookRotation(-normal) * Quaternion.Euler(0, 0, Random.Range(0, 360)));

        // auto destroy bullet hole after a delay
        Destroy(bulletHole, weaponShared.autoDestroyBulletHoleTime);
    }

    public void Shoot()
    {
        // didn't cooldown yet?
        if (weaponShared.currentCooldownTime > 0)
        {
            // can't shoot
            return;
        }

        weaponShared.currentReloadCooldownTime = 0;

        if (weaponSystem.currentWeaponType
            == WeaponType.Pistol
            && weaponShared.playerStats.bulletsInClip > 0)
        {
            weaponShared.playerStats.Fire("handgun");
        }
        else if (weaponSystem.currentWeaponType
                 == WeaponType.Shotgun
                 && weaponShared.playerStats.shellsInClip > 0)
        {
            weaponShared.playerStats.Fire("shotgun");
        }

        // play animation
        weaponShared.animator.Play(shootAnimationState, -1, 0);

        // reset cooldown time
        weaponShared.currentCooldownTime = cooldownTime;

        // play shoot sound
        PlayShootSound();

        // show muzzle flash
        SetMuzzleFlashVisible(true);

        // Keep track of time fired
        weaponShared.lastFireTime = Time.time;

        // Shoot each bullet
        for (int i = 0; i < bulletCount; i++)
        {
            ShootBullet();
        }
    }

    public void Reload()
    {
        // didn't cooldown yet?
        if (weaponShared.currentCooldownTime > 0)
        {
            // can't reload
            return;
        }

        // didn't cooldown yet?
        if (weaponShared.currentReloadCooldownTime > 0)
        {
            // can't reload
            return;
        }
        // TO-DO: Check if can reload
        //   - Check if not currently firing
        weaponShared.currentReloadCooldownTime = reloadCooldownTime;



        // Play weapon reload animation
        weaponShared.animator.Play(reloadAnimationState, -1, 0);

        // Play weapon reload sound
        PlayReloadSound();
    }

    private void PlayShootSound()
    {
        SoundUtils.PlaySound3D(shootSoundInstance, shootSoundEvent, gameObject);
    }

    private void PlayReloadSound()
    {
        SoundUtils.PlaySound3D(reloadSoundInstance, reloadSoundEvent, gameObject);
    }
}