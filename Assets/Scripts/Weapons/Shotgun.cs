using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Shotgun : MonoBehaviour
{
    // TODO: customize the layer mask so it doesn't hit things like trigger volumes
    [FormerlySerializedAs("layerMask")] [SerializeField]
    private LayerMask collisionLayerMask = Physics.DefaultRaycastLayers;

    [SerializeField] private Transform fpsCam;

    [SerializeField] private GameObject shotgunModel;

    [Min(1)] [SerializeField] private int bulletCount = 20;
    [Min(1)] [SerializeField] private int shellsShootCost = 2;
    [Min(0)] [SerializeField] private float cooldownTime = 1.4f;
    [Min(1)] [SerializeField] private float damagePerBullet = 2f;

    [Header("Bullet Hole")] [SerializeField]
    private GameObject bulletHolePrefab;

    [SerializeField] private float autoDestroyBulletHoleTime = 10f;

    [Header("Muzzle Flash")] [SerializeField]
    private GameObject muzzleFlash;

    [SerializeField] private float hideMuzzleFlashAfterTime = 0.1f;

    [Header("Bullet Spread")] [SerializeField] [Min(0f)] [Tooltip("Degrees for spread on y-axis (horizontal)")]
    private float maxSpreadDegreesY = 5f;

    [SerializeField] [Min(0f)] [Tooltip("Degrees for spread on x-axis (vertical)")]
    private float maxSpreadDegreesX = 5f;

    private PlayerStats playerStats;
    private float currentCooldownTime;

    // Sounds
    private FMOD.Studio.EventInstance shootSoundInstance;
    private FMOD.Studio.EventInstance reloadSoundInstance;

    private float lastFireTime;


    private void Awake()
    {
        playerStats = GetComponentInParent<PlayerStats>();

        // hide shotgun and muzzle flash to start
        SetVisible(false);
        SetMuzzleFlashVisible(false);
    }

    private void Update()
    {
        // update cooldown time
        currentCooldownTime -= Time.deltaTime;

        // hide muzzle flash after a delay
        if (muzzleFlash.activeSelf && Time.time > lastFireTime + hideMuzzleFlashAfterTime)
        {
            SetMuzzleFlashVisible(false);
        }
    }

    public void SetVisible(bool visible)
    {
        shotgunModel.SetActive(visible);
    }

    private void SetMuzzleFlashVisible(bool visible)
    {
        muzzleFlash.SetActive(visible);
    }

    private void CreateBulletHoleDecal(Vector3 point, Vector3 normal)
    {
        // spawn a bullet hole decal
        GameObject bulletHole = Instantiate(bulletHolePrefab, point + normal * Random.Range(0.001f, 0.002f),
            Quaternion.LookRotation(-normal) * Quaternion.Euler(0, 0, Random.Range(0, 360)));

        // auto destroy bullet hole after a delay
        Destroy(bulletHole, autoDestroyBulletHoleTime);
    }

    public void Shoot()
    {
        // didn't cooldown yet?
        if (currentCooldownTime > 0)
        {
            // can't shoot
            return;
        }

        // does the player have enough shells?
        if (!playerStats.UseShells(shellsShootCost))
        {
            // TODO: play out of ammo sound
            return;
        }

        // reset cooldown time
        currentCooldownTime = cooldownTime;

        // play shoot sound
        PlayShootSound();

        // play reload sound after a delay
        // TODO: tie this to an animation event
        // play it after a delay
        Invoke(nameof(PlayReloadSound), 0.2f);

        // show muzzle flash
        SetMuzzleFlashVisible(true);

        // Keep track of time fired
        lastFireTime = Time.time;

        // Shoot each bullet
        for (int i = 0; i < bulletCount; i++)
        {
            ShootBullet();
        }
    }

    private void PlayShootSound()
    {
        SoundUtils.PlaySound3D(shootSoundInstance, "Sfxs/Player/gun/Shoot", gameObject);
    }

    private void PlayReloadSound()
    {
        SoundUtils.PlaySound3D(reloadSoundInstance, "Sfxs/Player/gun/Reload", gameObject);
    }

    private void ShootBullet()
    {
        // get the camera's forward direction
        Vector3 fpsCamForward = fpsCam.forward;

        // calculate random spread
        float degreesX = Random.Range(-maxSpreadDegreesX, maxSpreadDegreesX);
        float degreesY = Random.Range(-maxSpreadDegreesY, maxSpreadDegreesY);

        // calculate trajectory
        Quaternion rotationX = Quaternion.AngleAxis(degreesX, fpsCam.right);
        Quaternion rotationY = Quaternion.AngleAxis(degreesY, fpsCam.up);
        Vector3 bulletTrajectory = rotationX * rotationY * fpsCamForward;

        // Raycast for hit
        const float range = 10000f; // max distance
        if (!Physics.Raycast(fpsCam.transform.position, bulletTrajectory, out var hit, range, collisionLayerMask))
        {
            // didn't hit so nothing to do
            return;
        }
        
        // log the collider layer
        Debug.Log($"Shotgun hit layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");

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
}