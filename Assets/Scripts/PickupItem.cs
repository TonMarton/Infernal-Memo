using UnityEngine;

public enum StatType
{
    Armor,
    Health,
    Bullets,
    Shells,
}

public class PickupItem : MonoBehaviour
{
    [SerializeField]
    private StatType statType;

    [SerializeField]
    private int amount = 10;
    
    [Header("Sound")]
    [SerializeField] private FMODUnity.EventReference pickupSoundEvent;

    // Sounds
    private FMOD.Studio.EventInstance pickupSoundInstance;

    private void OnTriggerEnter(Collider other)
    {
        PlayerStats playerStats = other.GetComponent<PlayerStats>();

        if (playerStats != null)
        {
            switch (statType)
            {
                case StatType.Armor:
                    Debug.Log("Picked up Armor");
                    playerStats.UpdateArmor(amount);
                    break;
                case StatType.Bullets:
                    Debug.Log("Picked up Bullets");
                    playerStats.UpdateBullets(amount);
                    break;
                case StatType.Shells:
                    Debug.Log("Picked up Shells");
                    playerStats.UpdateShells(amount);
                    break;
                case StatType.Health:
                    Debug.Log("Picked up Health");
                    playerStats.UpdateHealth(amount, gameObject);
                    break;
            }
            // pickup sound
            SoundUtils.PlaySound3D(ref pickupSoundInstance, pickupSoundEvent, gameObject); 
            
            gameObject.SetActive(false);
        }
    }
}
