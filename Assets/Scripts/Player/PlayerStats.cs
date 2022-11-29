using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int startingHealth = 100;
    [SerializeField] private int maxHealth = 100;

    [Header("Armor")]
    [SerializeField] private int startingArmor;
    [SerializeField] private int maxArmor = 100;

    [Header("Ammo - Pistol")]
    // pistol
    [SerializeField] private int startingBullets = 12;
    public int maxBullets = 99;
    [SerializeField] private int startingBulletsInClip = 12;
    public int maxBulletsInClip = 12;

    [Header("Ammo - Shotgun")]
    // shotgun
    [SerializeField] private int startingShells = 20;
    public int maxShells = 99;
    [SerializeField] private int startingShellsInClip = 3;
    public int maxShellsInClip = 3;

    [Header("UI")]
    [SerializeField] private DeathMenu deathMenu;
    [SerializeField] private HUD hud;

    [Header("Sound")]
    [SerializeField] private FMODUnity.EventReference hurtSoundEvent;
    [SerializeField] private FMODUnity.EventReference deathSoundEvent;

    // Sounds
    private FMOD.Studio.EventInstance hurtSoundInstance;
    private FMOD.Studio.EventInstance deathSoundInstance;

    // Stats
    private int health;
    private int armor;

    [HideInInspector] public int bullets;
    [HideInInspector] public int bulletsInClip;

    [HideInInspector] public int shells;
    [HideInInspector] public int shellsInClip;

    public bool isDead => health <= 0;

    public UnityEvent onDie;

    public GameObject causeOfDeath { get; private set; }

    private void Awake()
    {
        // initialize stats 
        health = startingHealth;
        armor = startingArmor;
        bullets = startingBullets;
        bulletsInClip = startingBulletsInClip;
        shells = startingShells;
        shellsInClip = startingShellsInClip;

        // set reference to HUD
        hud = GetComponentInChildren<HUD>();

        // initialize HUD
        hud.UpdateUIText("health", health);
        hud.UpdateUIText("armor", armor);
        hud.UpdateUIText("bulletsInClip", bulletsInClip);
        hud.UpdateUIText("bullets", bullets);
        hud.UpdateUIText("shellsInClip", shellsInClip);
        hud.UpdateUIText("shells", shells);
    }

    private void Update()
    {
        // Test take damage
        //if (Input.GetKeyDown(KeyCode.B))
        //{
        //    TakeDamage(10);
        //}

        // Test death
        //if (Input.GetKeyDown(KeyCode.B))
        //{
        //    TakeDamage(1000);
        //}
    }

    public void TakeDamage(int amount, GameObject sender)
    {
        int armorReducedAmount = amount - armor;
        if (armorReducedAmount != amount)
        {
            UpdateArmor(-amount);
        }
        if (armorReducedAmount > 0)
        {
            UpdateHealth(-armorReducedAmount, sender);
        }
    }

    //updates player health,
    //either when player takes damage or picks up a health item to heal themselves
    public void UpdateHealth(int amount, GameObject sender)
    {
        health += amount;

        if (amount < 0)
        {
            hud.AddDamageFlash(Mathf.Abs(amount) / 100.0f);
            if (health <= 0)
            {
                health = 0;
                causeOfDeath = sender;
                Die();
            }
            else
            {
                gameObject.GetComponent<Controller>().damageTaken = true;
                SoundUtils.PlaySound3D(ref hurtSoundInstance, hurtSoundEvent, gameObject);
            }
        }
        else
        {
            if (health > maxHealth)
            {
                health = maxHealth;
            }
        }

        hud.UpdateUIText("health", health);
    }

    public void UpdateArmor(int amount)
    {
        if (amount > 0)
        {
            armor = Mathf.Min(maxArmor, armor + amount);
        }
        else
        {
            armor = Mathf.Max(0, armor + amount);
        }
        hud.UpdateUIText("armor", armor);
    }

    public void UpdateBullets(int amount)
    {
        bullets = Mathf.Min(maxBullets, bullets + amount);
        hud.UpdateUIText("bullets", bullets);
    }

    public void UpdateShells(int amount)
    {
        shells = Mathf.Min(maxShells, shells + amount);
        hud.UpdateUIText("shells", shells);
    }

    private void Die()
    {
        // play death sound with Fmod
        SoundUtils.PlaySound3D(ref deathSoundInstance, deathSoundEvent, gameObject);

        // show the death menu 
        deathMenu.Show();

        // invoke event
        onDie.Invoke();
    }

    //fire while there are enough remaining bullets
    public void Fire(string gun)
    {
        if (gun == "handgun")
        {
            bulletsInClip--;
            hud.UpdateUIText("bulletsInClip", bulletsInClip);
            Debug.Log("Shot handgun");
        }
        else if (gun == "shotgun")
        {
            shellsInClip--;
            hud.UpdateUIText("shellsInClip", shellsInClip);
            Debug.Log("Shot shotgun");
        }
    }

    //reload while there are enough remaining bullets
    public void Reload(WeaponType gun)
    {
        if (gun == WeaponType.Pistol
            && bullets > 0)
        {

            int bulletsDesired = maxBulletsInClip - bulletsInClip;
            int addBullets = Mathf.Min(bulletsDesired, bullets);

            bulletsInClip += addBullets;
            bullets -= addBullets;

            //if (maxBullets >= bullets)
            //{
            //    if (bulletsInClip == 0)
            //    {
            //        bulletsInClip = maxBulletsInClip;
            //        bullets -= bulletsInClip;
            //    }
            //    else
            //    {
            //        int remainingAvailableSpace = maxBullets - bulletsInClip;
            //        bulletsInClip += remainingAvailableSpace;
            //        bullets -= remainingAvailableSpace;
            //    }
            //}
            //else if (maxBullets > bullets)
            //{
            //    bulletsInClip = bullets;
            //    bullets = 0;
            //}

            hud.UpdateUIText("bullets", bullets);
            hud.UpdateUIText("bulletsInClip", bulletsInClip);
        }
        else if (gun == WeaponType.Shotgun
            && shells > 0)
        {

            int shellsDesired = maxShellsInClip - shellsInClip;
            int addShells = Mathf.Min(shellsDesired, shells);

            shellsInClip += addShells;
            shells -= addShells;

            //if (maxShells >= shells)
            //{
            //    if (shellsInClip == 0)
            //    {
            //        shellsInClip = maxShellsInClip;
            //        shells -= shellsInClip;
            //    }
            //    else
            //    {
            //        int remainingAvailableSpace = maxShells - shellsInClip;
            //        shellsInClip += remainingAvailableSpace;
            //        shells -= remainingAvailableSpace;
            //    }
            //}
            //else if (maxShells > shells)
            //{
            //    shellsInClip = shells;
            //    shells = 0;
            //}

            hud.UpdateUIText("shells", shells);
            hud.UpdateUIText("shellsInClip", shellsInClip);
        }

    }
}