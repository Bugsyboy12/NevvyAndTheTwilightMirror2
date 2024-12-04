using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 10; // Damage dealt by the projectile
    public float lifespan = 5f; // Time before the projectile disappears if it doesn't hit anything
    public float destroyAfterHitDelay = 2f; // Time to wait before destroying the projectile after it hits

    private bool hasHit = false; // Tracks whether the projectile has already collided with something

    void Start()
    {
        Destroy(gameObject, lifespan); // Destroy the projectile after its lifespan
    }

    void OnTriggerEnter(Collider other)
    {
        // Only proceed if this is the first collision
        if (hasHit) return;

        // Check if the projectile hit an enemy
        if (other.CompareTag("Enemy"))
        {
            hasHit = true; // Mark the projectile as having collided
            EnemyAI enemy = other.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage); // Deal damage to the enemy
            }

            // Start the delayed destruction
            StartCoroutine(DestroyAfterDelay());
        }
    }

    private System.Collections.IEnumerator DestroyAfterDelay()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(destroyAfterHitDelay);

        // Destroy the projectile
        Destroy(gameObject);
    }
}
