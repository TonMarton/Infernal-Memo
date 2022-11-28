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
    [SerializeField] private Image crosshair;

    [SerializeField] private string levelClearMessage;
    [SerializeField] private string levelStillHasEnemiesMessage;

    Coroutine lastCoroutine;

    private void Start()
    {
        levelStatusMessageText.gameObject.SetActive(false);
    }

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

    public void ShowLevelClearMessage()
    {
        ShowLevelStatusMessageTextForFixedTime(5, levelClearMessage);
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