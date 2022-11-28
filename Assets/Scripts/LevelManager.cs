using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private int currentLevelIndex = 0;
    private GameObject currentLevel;
    private int enemyCount;
    public bool allowMovingLevels = false;

    private GameObject player;

    void Start()
    {
        player = GameObject.Find("Player");

        int index = 0;
        foreach (Transform child in transform)
        {
            if (index != 0)
            {
                child.gameObject.SetActive(false);
            }
            else
            {
                currentLevel = child.gameObject;
            }
            index += 1;
        }
        FindAllEnemies();
        if (enemyCount == 0)
        {
            allowMovingLevels = true;
        }
    }

    public void DecreaseEnemyCount() {
        if (--enemyCount == 0) {
            allowMovingLevels = true;
            Debug.Log("All enemies were killed");
            player.GetComponentInChildren<HUD>().ShowLevelClearMessage();
        } else
        {
            Debug.Log("There are " + enemyCount + " enemies left.");
        }
    }

    private void FindAllEnemies()
    {
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        Debug.Log("There are " + enemyCount + " enemies on this level");
    }


    public void ChangeToNextLevel(Vector3 destination,Quaternion rotationDiffernece)
    {
        if (allowMovingLevels)
        {
            ActivateNextLevel();
            player.GetComponent<Controller>().TeleportToPositionMaintainingRelativePosition(destination, rotationDiffernece);
            DeactivatePreviousLevel();
        }
        else {
            player.GetComponentInChildren<HUD>().ShowEnemiesOnLevelText();
        }
    }

    public void ActivateNextLevel()
    {
        allowMovingLevels = false;
        currentLevel = gameObject.transform.GetChild(++currentLevelIndex).gameObject;
        currentLevel.SetActive(true);
        FindAllEnemies();
    }

    public void DeactivatePreviousLevel()
    {
        gameObject.transform.GetChild(currentLevelIndex - 1).gameObject.SetActive(false);
    }
}