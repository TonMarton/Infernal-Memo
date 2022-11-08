using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
public class PlayerStats : MonoBehaviour
{
    [Header("Health")] [SerializeField] private int startingHealth = 100;
    [SerializeField] private int maxHealth = 100;

    [Header("Ammo")] [SerializeField] private int startingShells = 20;
    [SerializeField] private int maxShells = 99;

    [Header("UI")] [SerializeField] private DeathMenu deathMenu;
    [SerializeField] private HUD hud;

    [FormerlySerializedAs("hurtSound")] [Header("Sound")] [SerializeField]
    private FMODUnity.EventReference hurtSoundEvent;

    [FormerlySerializedAs("deathSound")] [SerializeField]
    private FMODUnity.EventReference deathSoundEvent;

    // Sounds
    private FMOD.Studio.EventInstance hurtSoundInstance;
    private FMOD.Studio.EventInstance deathSoundInstance;

    // Stats
    private int health;
    private int armor;
    private int shells;


    // Start is called before the first frame update
    private void Awake()
    {
        // initialize stats 
        health = startingHealth;
        shells = startingShells;

        // set reference to HUD
        hud = GetComponentInChildren<HUD>();

        // initialize HUD
        hud.ChangeHealthText(health);
        hud.ChangeArmorText(armor);
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


    public void GetShells(int count)
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
}