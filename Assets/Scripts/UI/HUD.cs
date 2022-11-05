using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class HUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI armorText;
    [SerializeField] private TextMeshProUGUI shellsText;

    private PlayerStats playerStats;

    private void Awake()
    {
        // hide cursor
        Cursor.visible = false;

        // store a reference to the player's PlayerStats component
        var player = GameObject.Find("Player");
        playerStats = player.GetComponent<PlayerStats>();
    }

    private void Update()
    {
        // update the HUD with the player's stats
        healthText.text = "Health: " + playerStats.health;
        armorText.text = "Armor: " + playerStats.armor;
        shellsText.text = "Shells: " + playerStats.shells;
    }
}