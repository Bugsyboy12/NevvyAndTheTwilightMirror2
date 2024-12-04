using UnityEngine;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float attackCooldown = 1f;
    public int damage = 10;
    public int maxHealth = 50;
    public Animator animator;
    public GameObject healthBarPrefab;
    public Vector3 healthBarOffset = new Vector3(0, 2, 0);

    private GameObject healthBarInstance;
    private Slider healthBar;
    private GameObject player;
    private Rigidbody rb;
    private Vector3 moveDirection;
    private bool isPlayerInRange = false;
    private float attackTimer = 0f;
    private int currentHealth;
    private bool isDead = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;

        if (healthBarPrefab != null)
        {
            healthBarInstance = Instantiate(healthBarPrefab, transform.position + healthBarOffset, Quaternion.identity);
            healthBar = healthBarInstance.GetComponentInChildren<Slider>();
            healthBarInstance.SetActive(false);
            healthBarInstance.transform.SetParent(null);
        }
    }

    void Update()
    {
        if (isDead || player == null) return;

        attackTimer -= Time.deltaTime;

        if (healthBarInstance != null)
        {
            healthBarInstance.transform.position = transform.position + healthBarOffset;
        }

        if (isPlayerInRange)
        {
            rb.linearVelocity = Vector3.zero;

            if (attackTimer <= 0)
            {
                Attack();
                attackTimer = attackCooldown;
            }
        }
        else
        {
            ChasePlayer();
        }
    }

    void FixedUpdate()
    {
        if (isDead || isPlayerInRange || player == null) return;

        rb.linearVelocity = moveDirection * moveSpeed;

        if (moveDirection != Vector3.zero)
            transform.forward = moveDirection;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    void ChasePlayer()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        direction.y = 0;
        moveDirection = direction;
    }

    void Attack()
    {
        Debug.Log("Enemy Attacked!");
        player.GetComponent<PlayerController>().TakeDamage(damage);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (healthBar != null)
        {
            healthBar.value = (float)currentHealth / maxHealth;

            if (healthBarInstance != null)
            {
                InputManager.Instance.ShowHealthBar(healthBarInstance, 10f);
            }
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;
        GetComponent<Collider>().enabled = false;

        if (healthBarInstance != null)
        {
            Destroy(healthBarInstance);
        }

        float deathAnimationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        Destroy(gameObject, deathAnimationLength);
    }
}
