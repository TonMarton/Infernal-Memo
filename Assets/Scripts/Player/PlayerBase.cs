using UnityEngine;

[DisallowMultipleComponent]
public class PlayerBase : MonoBehaviour
{
    public const string PLAYER_TAG = "Player";
    
    private void Awake()
    {
        // set the player tag
        gameObject.tag = PLAYER_TAG;
    }
}