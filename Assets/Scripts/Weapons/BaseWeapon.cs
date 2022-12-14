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
    [SerializeField] protected float maxDistance = 10000f;
    [SerializeField] protected GameObject bulletHolePrefab;
    
    private PlayerWeaponSystem weaponSystem;

    [Header("Muzzle Flash")]
    [SerializeField]
    private GameObject muzzleFlashObject;
    private ParticleSystem muzzleFlashEffect;

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
    private FMOD.Studio.EventInstance brickBulletImpactSoundInstance;


    [Header("Animation")]
    public string drawAnimationState;
    public string putAwayAnimationState;
    public string idleAnimationState;
    public string shootAnimationState;
    public string reloadAnimationState;

    private void Awake()
    {
        if (muzzleFlashObject != null)
        {
            muzzleFlashEffect = muzzleFlashObject.GetComponent<ParticleSystem>();
        }

        weaponSystem = GetComponent<PlayerWeaponSystem>();

        // hide shotgun and muzzle flash to start
        SetVisible(false);
    }

    public void StopReload()
    {
        SoundUtils.StopSound3D(reloadSoundInstance);
    }

    private void ShootBullet()
    {
        // get the camera's forward direction
        Vector3 fpsCamForward = weaponSystem.fpsCam.forward;

        // calculate random spread
        float degreesX = Random.Range(-maxSpreadDegreesX, maxSpreadDegreesX);
        float degreesY = Random.Range(-maxSpreadDegreesY, maxSpreadDegreesY);

        // calculate trajectory
        Quaternion rotationX = Quaternion.AngleAxis(degreesX, weaponSystem.fpsCam.right);
        Quaternion rotationY = Quaternion.AngleAxis(degreesY, weaponSystem.fpsCam.up);
        Vector3 bulletTrajectory = rotationX * rotationY * fpsCamForward;

        // Raycast for hit
        if (!Physics.Raycast(weaponSystem.fpsCam.transform.position, bulletTrajectory, out var hit, maxDistance, weaponSystem.collisionLayerMask, QueryTriggerInteraction.Ignore))
        {
            // didn't hit so nothing to do
            return;
        }

        // Not an enemy?
        if (hit.collider.gameObject.layer != LayerMask.NameToLayer(Enemy.EnemyHitboxLayer))
        {
            // Create a bullet hole
            CreateBulletHoleDecal(hit.point, hit.normal);

        }
        OnHit();

        // find the game object that was hit
        // and see if it was an enemy
        var hitObject = hit.collider.gameObject;

        //// log the tag of the object that was hit
        //if (!hitObject.transform.root.CompareTag(Enemy.EnemyTag))
        //{
        //    // wasn't an enemy so nothing to do
        //    return;
        //}

        // damage the enemy
        var enemy = hitObject.GetComponentInParent<Enemy>();

        if (enemy == null) return;
        var bloodImpact = Instantiate(weaponSystem.bloodImpactParticlePrefab, hit.point + hit.normal * weaponSystem.particleSpawnOffset, Quaternion.LookRotation(hit.normal));
        SoundUtils.PlaySound3DParameter(ref brickBulletImpactSoundInstance, weaponSystem.brickBulletImpactSoundEvent, bloodImpact, "Bullet_impacts", 0, Mathf.Clamp(1f / bulletCount, 0.3f, 1f));
        Destroy(bloodImpact, weaponSystem.autoDestroyParticleTime);
        var enemyStats = enemy.GetComponent<EnemyStats>();
        enemyStats.TakeDamage(damagePerBullet, knockback: null);
    }

    public void SetVisible(bool visible)
    {
        model.SetActive(visible);
    }

    public int impactParamIndex = 2;
    private void CreateBulletHoleDecal(Vector3 point, Vector3 normal)
    {
        var bulletImpact = Instantiate(weaponSystem.surfaceImpactParticlePrefab, point + normal * weaponSystem.particleSpawnOffset * 1.5f, Quaternion.LookRotation(normal)); ;
        Destroy(bulletImpact, weaponSystem.autoDestroyParticleTime);

        // spawn a bullet hole decal
        GameObject prefabToSpawn = bulletHolePrefab;
        if(prefabToSpawn==null)
        {
            prefabToSpawn=weaponSystem.bulletHolePrefab;
        }
        GameObject bulletHole = Instantiate(prefabToSpawn, point + normal * Random.Range(0.001f, 0.002f),
            Quaternion.LookRotation(-normal) * Quaternion.Euler(0, 0, Random.Range(0, 360)));

        // play bullet impact sound
        // to-do: play different sound depending on material

        SoundUtils.PlaySound3DParameter(ref brickBulletImpactSoundInstance, weaponSystem.brickBulletImpactSoundEvent, bulletImpact, "Bullet_impacts", impactParamIndex, Mathf.Clamp(1f / bulletCount, 0.3f, 1f));

        // auto destroy bullet hole after a delay
        Destroy(bulletHole, weaponSystem.autoDestroyBulletHoleTime);
    }

    public void Shoot()
    {
        // didn't cooldown yet?
        if (weaponSystem.currentCooldownTime > 0)
        {
            // can't shoot
            return;
        }

        weaponSystem.currentReloadCooldownTime = 0;

        if (weaponSystem.currentWeaponType
            == WeaponType.Pistol
            && weaponSystem.playerStats.bulletsInClip > 0)
        {
            weaponSystem.playerStats.Fire("handgun");
        }
        else if (weaponSystem.currentWeaponType
                 == WeaponType.Shotgun
                 && weaponSystem.playerStats.shellsInClip > 0)
        {
            weaponSystem.playerStats.Fire("shotgun");
        }

        // play animation
        if (!string.IsNullOrEmpty(shootAnimationState))
        {
            weaponSystem.armsAnimator.Play(shootAnimationState, -1, 0);
        }

        // reset cooldown time
        weaponSystem.currentCooldownTime = cooldownTime;

        // stop reloading sound
        StopReload();

        // play shoot sound
        PlayShootSound();

        // play muzzle flash
        PlayMuzzleFlash();

        // Keep track of time fired
        weaponSystem.lastFireTime = Time.time;

        // Shoot each bullet
        for (int i = 0; i < bulletCount; i++)
        {
            ShootBullet();
        }
    }

    protected virtual void OnHit()
    { 
    }

    protected virtual void PlayMuzzleFlash() {
        muzzleFlashEffect.Play();
    }

    public void Reload()
    {
        // didn't cooldown yet?
        if (weaponSystem.currentCooldownTime > 0)
        {
            // can't reload
            return;
        }

        // didn't cooldown yet?
        if (weaponSystem.currentReloadCooldownTime > 0)
        {
            // can't reload
            return;
        }
        // TO-DO: Check if can reload
        //   - Check if not currently firing
        weaponSystem.currentReloadCooldownTime = reloadCooldownTime;



        // Play weapon reload animation
        weaponSystem.armsAnimator.Play(reloadAnimationState, -1, 0);

        // Play weapon reload sound
        PlayReloadSound();
    }

    private void PlayShootSound()
    {
        SoundUtils.PlaySound3D(ref shootSoundInstance, shootSoundEvent, gameObject);
    }

    private void PlayReloadSound()
    {
        SoundUtils.PlaySound3D(ref reloadSoundInstance, reloadSoundEvent, gameObject);
    }
}