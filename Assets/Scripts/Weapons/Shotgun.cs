using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    // TODO: customize the layer mask so it doesn't hit things like trigger volumes
    [SerializeField] private LayerMask layerMask = Physics.DefaultRaycastLayers;
    [SerializeField] private Transform fpsCam;
    [SerializeField] private int bulletCount = 20;
    [SerializeField] private GameObject bulletHolePrefab;
    [SerializeField] private GameObject muzzleFlash;

    [SerializeField] [Min(0f)] [Tooltip("Degrees for spread on y-axis (horizontal)")]
    private float maxSpreadDegreesY = 5f;

    [SerializeField] [Min(0f)] [Tooltip("Degrees for spread on x-axis (vertical)")]
    private float maxSpreadDegreesX = 5f;

    private float lastFireTime;

    private void Start()
    {
        muzzleFlash.gameObject.SetActive(false);
    }

    private void Update()
    {
        // hide muzzle flash after a delay
        if (muzzleFlash.activeSelf && Time.time > lastFireTime + 0.1f)
        {
            muzzleFlash.SetActive(false);
        }
    }

    private void CreateBulletHole(Vector3 point, Vector3 normal)
    {
        GameObject bulletHole = Instantiate(bulletHolePrefab, point + normal * Random.Range(0.001f, 0.002f),
            Quaternion.LookRotation(-normal) * Quaternion.Euler(0, 0, Random.Range(0, 360)));
        Destroy(bulletHole, 10f);
    }

    public void Shoot()
    {
        // TODO: Play fire sound

        // Enable muzzle flash
        muzzleFlash.gameObject.SetActive(true);
        // Keep track of time fired
        lastFireTime = Time.time;

        // Create multiple bullets
        for (int i = 0; i < bulletCount; i++)
        {
            Vector3 fpsCamForward = fpsCam.forward;
            // Spread functionality
            float degreesX = Random.Range(-maxSpreadDegreesX, maxSpreadDegreesX);
            float degreesY = Random.Range(-maxSpreadDegreesY, maxSpreadDegreesY);
            Quaternion rotationX = Quaternion.AngleAxis(degreesX, fpsCam.right);
            Quaternion rotationY = Quaternion.AngleAxis(degreesY, fpsCam.up);
            Vector3 bulletTrajectory = rotationX * rotationY * fpsCamForward;
            //Vector3 bulletTrajectory = , Random.Range(-maxSpreadDegreesY, maxSpreadDegreesY), 0); * fpsCamForward;

            RaycastHit hit; // hit information
            float range = 10000f; // max distance
            // Raycast
            if (Physics.Raycast(fpsCam.transform.position, bulletTrajectory, out hit, range, layerMask))
            {
                // Bullet hole when hitting a wall
                CreateBulletHole(hit.point, hit.normal);

                // To-do: Damage enemy
            }
        }
    }
}