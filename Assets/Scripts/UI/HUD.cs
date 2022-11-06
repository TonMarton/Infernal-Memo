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
        // hide the default Unity engine cursor
        UnityEngine.Cursor.visible = false;
        
        // hide cursor by default
        SetCrossHairVisible(false);
    }

    public void SetCrossHairVisible(bool show)
    {
        // TODO: re-enable this, doesn't work
        // crossHair.visible = show;
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