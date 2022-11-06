using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    // TODO: customize the layer mask so it doesn't hit things like trigger volumes
    [SerializeField] private LayerMask layerMask = Physics.DefaultRaycastLayers;
    [SerializeField] private Transform fpsCam;

    [SerializeField] private GameObject shotgunModel;

    [Min(1)] [SerializeField] private int bulletCount = 20;

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

    private float lastFireTime;

    private void Awake()
    {
        // hide shotgun and muzzle flash to start
        SetVisible(false);
        SetMuzzleFlashVisible(false);
    }

    private void Update()
    {
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
        // TODO: Play fire sound

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

        const float range = 10000f; // max distance
        // Raycast for hit
        if (Physics.Raycast(fpsCam.transform.position, bulletTrajectory, out var hit, range, layerMask))
        {
            // Create a bullet hole when hitting a wall
            CreateBulletHoleDecal(hit.point, hit.normal);

            // To-do: Damage enemy
        }
    }
}