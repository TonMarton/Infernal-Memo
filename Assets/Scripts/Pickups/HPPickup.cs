using UnityEngine;

public class HPPickup : MonoBehaviour
{
    [SerializeField]
    private int HP = 10;

    private void OnTriggerEnter(Collider other)
    {
        PlayerStats playerStats = other.GetComponent<PlayerStats>();

        if (playerStats != null) {
            playerStats.Heal(HP);
            gameObject.SetActive(false);
        }
    }
}
