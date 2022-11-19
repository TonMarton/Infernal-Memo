using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int startingHealth = 100;
    [SerializeField] private int maxHealth = 100;

    [Header("Ammo - Pistol")]
    // pistol
    [SerializeField] private int startingBullets = 12;
    [SerializeField] private int maxBullets = 99;
    [SerializeField] private int startingBulletsInClip = 12;
    [SerializeField] private int maxBulletsInClip = 12;

    [Header("Ammo - Shotgun")]
    // shotgun
    [SerializeField] private int startingShells = 20;
    [SerializeField] private int maxShells = 99;
    [SerializeField] private int startingShellsInClip = 3;
    [SerializeField] private int maxShellsInClip = 3;

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

    private int bullets;
    private int bulletsInClip;

    private int shells;
    private int shellsInClip;

    private void Awake()
    {
        // initialize stats 
        health = startingHealth;
        bullets = startingBullets;
        bulletsInClip = startingBulletsInClip;
        shells = startingShells;
        shellsInClip = startingShellsInClip;

        // set reference to HUD
        hud = GetComponentInChildren<HUD>();

        // initialize HUD
        hud.ChangeHealthText(health);
        hud.ChangeArmorText(armor);
        hud.ChangeBulletsInClipText(bulletsInClip);
        hud.ChangeBulletsText(bullets);
        hud.ChangeShellsInClipText(shellsInClip);
        hud.ChangeShellsText(shells);
    }

    public void TakeDamage(int damage)
    {
        // take damage
        health -= damage;

        // update HUD
        hud.ChangeHealthText(health);

        if (health <= 0)
        {
            Die();
            return;
        }

        // shake screen
        gameObject.GetComponent<Controller>().damageTaken = true;

        // play hurt sound
        SoundUtils.PlaySound3D(hurtSoundInstance, hurtSoundEvent, gameObject);
    }

    public void Heal(int healing)
    {
        health = Mathf.Min(healing + health, maxHealth);

        // update HUD
        hud.ChangeHealthText(health);
    }

    private void Die()
    {
        // TODO: play death sound with Fmod

        // show the death menu 
        deathMenu.Show();
    }

    public bool UseBullets(int count)
    {
        if (bullets < count)
        {
            // didn't have enough ammo
            return false;
        }

        // use the bullets
        bullets -= count;

        // update HUD
        hud.ChangeBulletsText(bullets);

        // had enough ammo
        return true;
    }

    public bool UseBulletsInClip(int count)
    {
        if (bulletsInClip < count)
        {
            // didn't have enough ammo in clip
            return false;
        }

        // use bullets in clip
        bulletsInClip -= count;

        // update HUD
        hud.ChangeBulletsInClipText(bulletsInClip);

        // had enough ammo in clip
        return true;
    }

    public void AddBullets(int count)
    {
        // are we at max bullets?
        if (bullets == maxBullets)
        {
            // then do nothing
            return;
        }

        // add bullets
        bullets = Mathf.Min(bullets + count, maxBullets);

        // TODO: play shell pickup sound

        // update HUD
        hud.ChangeBulletsText(bullets);
    }

    public bool AddBulletsInClip(int count)
    {
        // are we at max bullets in clip?
        if (bulletsInClip == maxBulletsInClip)
        {
            // could not add further ammo to clip
            return false;
        }

        // add bullets to clip
        bulletsInClip = Mathf.Min(bulletsInClip + count, maxBulletsInClip);

        // update HUD
        hud.ChangeBulletsInClipText(bullets);

        return true;
    }

    public bool UseShells(int count)
    {
        if (shells < count)
        {
            // didn't have enough ammo
            return false;
        }

        // use the shells
        shells -= count;

        // update HUD
        hud.ChangeShellsText(shells);

        // had enough ammo
        return true;
    }

    public bool UseShellsInClip(int count)
    {
        if (shellsInClip < count)
        {
            // didn't have enough ammo in clip
            return false;
        }

        // use shells in clip
        shellsInClip -= count;

        // update HUD
        hud.ChangeShellsInClipText(shellsInClip);

        // had enough ammo in clip
        return true;
    }

    public void AddShells(int count)
    {
        // are we at max shells?
        if (shells == maxShells)
        {
            // then do nothing
            return;
        }

        // add shells
        shells = Mathf.Min(shells + count, maxShells);

        // TODO: play shell pickup sound

        // update HUD
        hud.ChangeShellsText(shells);
    }

    public bool AddShellsInClip(int count)
    {
        // are we at max shells in clip?
        if (shellsInClip == maxShellsInClip)
        {
            // could not add further ammo to clip
            return false;
        }

        // add shells to clip
        shellsInClip = Mathf.Min(shellsInClip + count, maxShellsInClip);

        // update HUD
        hud.ChangeShellsInClipText(shells);

        return true;
    }
}