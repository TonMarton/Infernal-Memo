using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[DisallowMultipleComponent]
public class HUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelStatusMessageText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI armorText;
    [SerializeField] private TextMeshProUGUI bulletsText;
    [SerializeField] private TextMeshProUGUI bulletsInClipText;
    [SerializeField] private TextMeshProUGUI shellsText;
    [SerializeField] private TextMeshProUGUI shellsInClipText;
    [SerializeField] private Image staplerSelected;
    [SerializeField] private Image pistolSelected;
    [SerializeField] private Image shotgunSelected;
    [SerializeField] private Image crosshair;

    [SerializeField] private string levelClearMessage;
    [SerializeField] private string levelStillHasEnemiesMessage;
    [SerializeField] private string gameWonMessage;

    [SerializeField] private Image damageFlash;
    [SerializeField] private float damageFlashFadeSpeed = 5f;

    Coroutine lastCoroutine;
    float damageFlashAlpha;

    public void SetSelectedWeapon(WeaponType weaponType)
    {
        staplerSelected.gameObject.SetActive(weaponType == WeaponType.Stapler);
        pistolSelected.gameObject.SetActive(weaponType == WeaponType.Pistol);
        shotgunSelected.gameObject.SetActive(weaponType == WeaponType.Shotgun);
    }

    private void Start()
    {
        levelStatusMessageText.gameObject.SetActive(false);
    }

    public void AddDamageFlash(float amount)
    {
        damageFlashAlpha += amount;
        damageFlashAlpha = Mathf.Clamp(damageFlashAlpha, 0, 0.5f);
    }

    private void SetDamageFlashAlpha(float a)
    {
        Color color = damageFlash.color;
        color.a = a;
        damageFlash.color = color;
    }

    private void Awake()
    {
        // hide the default Unity engine cursor
        UnityEngine.Cursor.visible = false;
        
        // hide cursor by default
        SetCrossHairVisible(false);

        // ensure damage flash alpha is 0
        damageFlashAlpha = 0;
        SetDamageFlashAlpha(damageFlashAlpha);
    }

    private void Update()
    {
        // fade damage flash
        if (damageFlashAlpha > 0)
        {
            damageFlashAlpha -= Time.deltaTime * damageFlashFadeSpeed;
            if (damageFlashAlpha < 0)
            {
                damageFlashAlpha = 0;
            }
            SetDamageFlashAlpha(damageFlashAlpha);
        }
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

    public void ShowLevelClearMessage()
    {
        ShowLevelStatusMessageTextForFixedTime(5, levelClearMessage);
    }

    public void ShowGameWonMessage()
    {
        ShowLevelStatusMessageTextForFixedTime(5, gameWonMessage);
    }

    public void ShowEnemiesOnLevelText()
    {
        ShowLevelStatusMessageTextForFixedTime(5, levelStillHasEnemiesMessage);
    }

    private void ShowLevelStatusMessageTextForFixedTime(float duration, string message)
    {
        if (lastCoroutine != null)
        {
            StopCoroutine(lastCoroutine);
        }
        lastCoroutine = StartCoroutine(StartLevelStatusMessageCoroutine(duration, message));
    }

    private IEnumerator StartLevelStatusMessageCoroutine(float duration, string message)
    {
        levelStatusMessageText.text = message;

        float elapsedTime = 0f;
        levelStatusMessageText.gameObject.SetActive(true);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        levelStatusMessageText.gameObject.SetActive(false);
    }
}