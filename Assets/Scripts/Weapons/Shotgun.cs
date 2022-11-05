using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
<<<<<<< HEAD
    public Transform fpsCam;
    public AudioSource fireSound;
    public AudioSource reloadSound;
    [Min(0f)] [Tooltip("Degrees for spread on y-axis (horizontal)")] public float maxSpreadDegreesY = 5f;
    [Min(0f)] [Tooltip("Degrees for spread on x-axis (vertical)")] public float maxSpreadDegreesX = 5f;
    public LayerMask layerMask = Physics.DefaultRaycastLayers;
    public int bulletCount = 20;
    public GameObject bulletHolePrefab;
    public GameObject muzzleFlash;
    float lastFireTime;

    public void CreateBulletHole(Vector3 point, Vector3 normal)
=======
    // Start is called before the first frame update
    void Start()
>>>>>>> parent of 146afeb (Basic shotgun functionality)
    {
        
    }

    // Update is called once per frame
    void Update()
    {
<<<<<<< HEAD
        muzzleFlash.gameObject.SetActive(false);
    }

    public void Shoot()
    {
        // Play fire sound
        fireSound.Play();
        // Enable muzzle flash
        muzzleFlash.gameObject.SetActive(true);
        // Keep track of time fired
        lastFireTime = Time.time;

        // Create multiple bullets
        for (int i = 0; i < bulletCount; i++)
        {
            Vector3 fpsCamForward = fpsCam.forward;
            // Spread functionality
            Vector3 bulletTrajectory = Quaternion.Euler(Random.Range(-maxSpreadDegreesX, maxSpreadDegreesX), Random.Range(-maxSpreadDegreesY, maxSpreadDegreesY), 0) * fpsCamForward;

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
=======
        
>>>>>>> parent of 146afeb (Basic shotgun functionality)
    }
}
