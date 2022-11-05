using UnityEngine;

public class StaplerHitbox : MonoBehaviour
{
    [SerializeField] private float damage = 10f;

    // on trigger collision
    private void OnTriggerEnter(Collider other)
    {
        // wasn't an enemy?
        if (!other.gameObject.CompareTag("Enemy"))
        {
            // ignore
            return;
        }

        // do damage to the enemy stats
        other.gameObject.GetComponent<EnemyStats>().TakeDamage(damage);

        // disable the hitbox
        StopCanHit();
    }

    public void StopCanHit()
    {
        // disable the hitbox
        gameObject.SetActive(false);
    }

    public void StartCanHit()
    {
        // enable the hitbox
        gameObject.SetActive(true);
    }

    public bool CanHit()
    {

        return gameObject.activeSelf;
    }
}