using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    public Transform fpsCam;
    public AudioSource fireSound;
    public AudioSource reloadSound;
    [Min(0f)] public float maxSpreadDegreesX = 5f;
    [Min(0f)] public float maxSpreadDegreesY = 5f;
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
        fireSound.Play();
        muzzleFlash.gameObject.SetActive(true);
        lastFireTime = Time.time;

        for (int i = 0; i < bulletCount; i++)
        {
            Vector3 fpsCamForward = fpsCam.forward;
            Vector3 bulletTrajectory = Quaternion.Euler(Random.Range(-maxSpreadDegreesX, maxSpreadDegreesX), Random.Range(-maxSpreadDegreesY, maxSpreadDegreesY), 0) * fpsCamForward;

            RaycastHit hit;
            float range = 10000f;
            if (Physics.Raycast(fpsCam.transform.position, bulletTrajectory, out hit, range, layerMask))
            {
                // Bullet hole when hitting a wall
                CreateBulletHole(hit.point, hit.normal);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Shoot();
        }

        if (muzzleFlash.activeSelf && Time.time > lastFireTime + 0.1f)
        {
            muzzleFlash.SetActive(false);
        }
    }
}
