using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

[DisallowMultipleComponent]
public class HUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI armorText;
    [SerializeField] private TextMeshProUGUI bulletsText;
    [SerializeField] private TextMeshProUGUI bulletsInClipText;
    [SerializeField] private TextMeshProUGUI shellsText;
    [SerializeField] private TextMeshProUGUI shellsInClipText;
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
        healthText.text = "" + health;
    }

    public void ChangeArmorText(int armor)
    {
        armorText.text = "" + armor;
    }

    public void ChangeBulletsText(int bullets)
    {
        bulletsText.text = "" + bullets;
    }

    public void ChangeBulletsInClipText(int bulletsInClip)
    {
        bulletsInClipText.text = "" + bulletsInClip;
    }

    public void ChangeShellsText(int shells)
    {
        shellsText.text = "" + shells;
    }

    public void ChangeShellsInClipText(int shellsInClip)
    {
        shellsInClipText.text = "" + shellsInClip;
    }
}