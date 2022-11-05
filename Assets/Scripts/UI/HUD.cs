using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

[DisallowMultipleComponent]
public class HUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI armorText;
    [SerializeField] private TextMeshProUGUI shellsText;
    [SerializeField] private Image crossHair;

    private void Awake()
    {
        // hide cursor
        UnityEngine.Cursor.visible = false;
    }

    public void ToggleCrossHair(bool show)
    {
        crossHair.visible = show;
    }

    public void ChangeHealthText(int health)
    {
        healthText.text = "Health: " + health;
    }

    public void ChangeArmorText(int armor)
    {
        armorText.text = "Armor: " + armor;
    }

    public void ChangeShellsText(int shells)
    {
        shellsText.text = "Shells: " + shells;
    }
}