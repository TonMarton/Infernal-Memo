using UnityEngine;

public enum StatType
{
    Armour,
    Health,
    Ammo,
}

public class PickupItem : MonoBehaviour
{
    [SerializeField]
    private StatType statType;

    [SerializeField]
    private int amount = 10;

    private void OnTriggerEnter(Collider other)
    {
        PlayerStats playerStats = other.GetComponent<PlayerStats>();

        if (playerStats != null)
        {
            switch (statType)
            {
                case StatType.Armour:
                    Debug.Log("Armour Pickups not implemented");
                    break;
                case StatType.Ammo:
                    Debug.Log("Picked up Ammo");
                    playerStats.UpdateAmmo(amount);
                    break;
                case StatType.Health:
                    Debug.Log("Picked up Health");
                    playerStats.UpdateHealth(amount);
                    break;
            }
            gameObject.SetActive(false);
        }
    }
}
