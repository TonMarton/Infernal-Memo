using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    public Transform fpsCam;
    [Min(0f)] [Tooltip("Degrees for spread on y-axis (horizontal)")] public float maxSpreadDegreesY = 5f;
    [Min(0f)] [Tooltip("Degrees for spread on x-axis (vertical)")] public float maxSpreadDegreesX = 5f;
    public LayerMask layerMask = Physics.DefaultRaycastLayers;
    public int bulletCount = 20;
    public GameObject bulletHolePrefab;
    public GameObject muzzleFlash;
    float lastFireTime;

    public void CreateBulletHole(Vector3 point, Vector3 normal)
    {
        GameObject bulletHole = Instantiate(bulletHolePrefab, point + normal * Random.Range(0.001f, 0.002f), Quaternion.LookRotation(-normal) * Quaternion.Euler(0, 0, Random.Range(0, 360)));
        Destroy(bulletHole, 10f);
    }

    private void Start()
    {
        muzzleFlash.gameObject.SetActive(false);
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

    private void Update()
    {
        // To-do: Hold down mouse button and wait a specified interval between shots
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Shoot();
        }

        // Muzzle flash delay
        if (muzzleFlash.activeSelf && Time.time > lastFireTime + 0.1f)
        {
            muzzleFlash.SetActive(false);
        }
    }
}
