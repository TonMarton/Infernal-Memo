using UnityEngine;
using UnityEngine.Serialization;

public class PlayerStaplerAttack : MonoBehaviour, IPlayerAttack
{
    [SerializeField] private StaplerHitbox staplerHitbox;
    [SerializeField] private float startCanHitTime = 0.2f;
    [SerializeField] private float stopCanHitTime = 0.8f;

    private void Awake()
    {
        // disable the hitbox by default
        staplerHitbox.StopCanHit();
    }

    public void Attack()
    {
        // start can hit after a delay
        Invoke(nameof(StartCanHit), startCanHitTime);

        // stop can hit after a delay
        Invoke(nameof(StopCanHit), stopCanHitTime);
    }

    private void StartCanHit()
    {
        // delegate to the hitbox
        staplerHitbox.StartCanHit();
    }

    private void StopCanHit()
    {
        // delegate to the hitbox
        staplerHitbox.StopCanHit();
    }

    // draw GUI of the hitbox volume if it's active
    private void OnGUI()
    {
        // hitbox can't hit?
        if (!staplerHitbox.CanHit())
        {
            // don't draw the gizmo
            return;
        }

        // draw a cube at the hitbox position
        var hitboxPosition = staplerHitbox.transform.position;
        // get size of the hitbox volume's transform
        var hitboxSize = staplerHitbox.transform.lossyScale;
        var hitboxRect = new Rect(hitboxPosition.x, hitboxPosition.y, hitboxSize.x, hitboxSize.y);
        GUI.Box(hitboxRect, "Hitbox");
    }
}

public interface IPlayerAttack
{
    public void Attack();
}