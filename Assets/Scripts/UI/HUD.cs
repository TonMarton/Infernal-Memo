using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class HUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI armorText;
    [SerializeField] private TextMeshProUGUI bulletsText;
    [SerializeField] private TextMeshProUGUI bulletsInClipText;
    [SerializeField] private TextMeshProUGUI shellsText;
    [SerializeField] private TextMeshProUGUI shellsInClipText;
    [SerializeField] private Image crosshair;

    private void Awake()
    {
        // hide the default Unity engine cursor
        UnityEngine.Cursor.visible = false;
        
        // hide cursor by default
        SetCrossHairVisible(false);
    }

    public void SetCrossHairVisible(bool show)
    {
        crosshair.enabled = show;
    }

    //update any UI text from one method,
    //simply add a new if check to update what UI was updated
    public void UpdateUIText(string textType, 
                             int value)
    {
        if (textType == "health")
        {
            healthText.text = value.ToString();
        }
        else if (textType == "armor")
        {
            armorText.text = value.ToString();
        }
        else if (textType == "bullets")
        {
            bulletsText.text = value.ToString();
        }
        else if (textType == "bulletsInClip")
        {
            bulletsInClipText.text = value.ToString();
        }
        else if (textType == "shells")
        {
            shellsText.text = value.ToString();
        }
        else if (textType == "shellsInClip")
        {
            shellsInClipText.text = value.ToString();
        }
        else
        {
            Debug.LogError("Error: Invalid text type!");
        }
    }
}