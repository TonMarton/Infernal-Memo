using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class WeaponSharedComponent : MonoBehaviour
{
    // TODO: customize the layer mask so it doesn't hit things like trigger volumes
    [FormerlySerializedAs("layerMask")]
    [SerializeField]
    public LayerMask collisionLayerMask = Physics.DefaultRaycastLayers;

    [SerializeField]
    public Transform fpsCam;

    [SerializeField]
    public Animator animator;

    [Header("Bullet Hole")]
    [SerializeField]
    public GameObject bulletHolePrefab;

    [SerializeField]
    public float autoDestroyBulletHoleTime = 10f;

    public PlayerStats playerStats { get; private set; }

    public float currentCooldownTime { get; set; }
    public float lastFireTime { get; set; }


    private void Awake()
    {
        playerStats = GetComponentInParent<PlayerStats>();
    }
}
