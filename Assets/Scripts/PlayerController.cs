using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float dodgeSpeed = 10f;
    public float dodgeDuration = 0.2f;
    public float dodgeCooldown = 1.0f;
    public float attackCooldown = 0.5f;
    public int maxHealth = 100;

    public Slider healthSlider; // Reference to the health slider UI element
    public GameObject gameOverText; // Reference to the "Game Over" text GameObject
    public GameObject projectilePrefab; // Assign your projectile prefab in the Inspector
    public Transform projectileSpawnPoint; // Point from which the projectile is spawned
    public float projectileSpeed = 10f;
    public LayerMask groundLayer; // Layer mask to filter for ground

    private Rigidbody rb;
    private Vector3 moveInput;
    private Vector3 mouseDirection;
    private bool isDodging = false;
    private float dodgeTime;
    private float lastDodgeTime = -Mathf.Infinity; // Tracks the last time dodge was used
    private float attackTimer = 0f;
    private int currentHealth;
    private bool isGameOver = false; // Tracks if the game is over

    private Animator animator; // Reference to the Animator

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        // Initialize the health slider
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        // Ensure the Game Over text is hidden
        if (gameOverText != null)
        {
            gameOverText.SetActive(false);
        }
    }

    void Update()
    {
        if (isGameOver)
        {
            return; // Stop all player input if the game is over
        }

        if (isDodging)
        {
            dodgeTime -= Time.deltaTime;
            if (dodgeTime <= 0)
            {
                isDodging = false;
            }
            return;
        }

        // Movement Input
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        moveInput = new Vector3(moveX, 0, moveZ).normalized;

        // Mouse Direction (with Layer Mask)
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            mouseDirection = (hit.point - transform.position).normalized;
            mouseDirection.y = 0; // Keep movement on the horizontal plane
        }

        // Dodge Mechanic
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDodging && Time.time >= lastDodgeTime + dodgeCooldown)
        {
            isDodging = true;
            dodgeTime = dodgeDuration;
            lastDodgeTime = Time.time; // Update last dodge time

            // Trigger the dodge animation immediately
            animator.SetTrigger("Dodge");

            // Apply the dodge movement
            rb.linearVelocity = moveInput * dodgeSpeed;
        }

        // Attack
        attackTimer -= Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && attackTimer <= 0)
        {
            Attack();
            attackTimer = attackCooldown;
        }

        // Animation Transitions
        HandleAnimationTransitions();
    }

    void FixedUpdate()
    {
        if (isGameOver) return;

        if (!isDodging)
        {
            rb.linearVelocity = moveInput * moveSpeed;

            // Face the movement direction
            if (moveInput != Vector3.zero)
                transform.forward = moveInput;
        }
    }

    void Attack()
    {
        transform.forward = mouseDirection; // Face mouse direction when attacking

        // Trigger Attack Animation
        animator.SetTrigger("Attack");

        // Spawn and launch projectile
        if (projectilePrefab != null && projectileSpawnPoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
            Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
            if (projectileRb != null)
            {
                projectileRb.linearVelocity = mouseDirection * projectileSpeed;
            }
        }

        Debug.Log("Player Attacked!");
    }

    void HandleAnimationTransitions()
    {
        if (!isDodging && moveInput.magnitude > 0)
        {
            // Player is moving
            animator.SetBool("IsRunning", true);
            animator.SetBool("IsIdle", false);
        }
        else if (!isDodging)
        {
            // Player is idle
            animator.SetBool("IsRunning", false);
            animator.SetBool("IsIdle", true);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isGameOver) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0); // Prevent health from going below zero
        Debug.Log("Player Health: " + currentHealth);

        // Update the health slider
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player Died");

        isGameOver = true;

        // Show "Game Over" text
        if (gameOverText != null)
        {
            gameOverText.SetActive(true);
        }

        // Stop all gameplay
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        // Restart the game or load the current scene
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
