using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Transform playerTransform;
    private EnemyStats stats;
    private Animator enemyAnimator; 
    private Rigidbody2D rb; 

    [Header("Hareket Ayarları")]
    public float stopDistance = 1.5f;   

    [Header("Saldırı Ayarları")]
    public float attackInterval = 1.5f; 
    private float attackTimer = 0f;
    public int damageAmount = 5;        

    [HideInInspector] 
    public bool isDead = false;         

    [Header("Ses Ayarları")]
    public AudioClip walkLoopSound; 
    public AudioClip enemyAttackSound; 
    private AudioSource audioSource;   

    private float moveGroanTimer = 0f;
    private float nextMoveGroanTime = 0f;

    void Start()
    {
        stats = GetComponent<EnemyStats>();
        enemyAnimator = GetComponent<Animator>(); 
        audioSource = GetComponent<AudioSource>(); 
        rb = GetComponent<Rigidbody2D>(); 

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;

        attackTimer = attackInterval; 
        nextMoveGroanTime = Random.Range(1f, 3f);
    }

    void Update()
    {
        if (isDead || playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer > stopDistance)
        {
            MoveTowardsPlayer();
            attackTimer = attackInterval; 

            moveGroanTimer += Time.deltaTime;
            if (moveGroanTimer >= nextMoveGroanTime)
            {
                moveGroanTimer = 0f;
                nextMoveGroanTime = Random.Range(3f, 7f); 

                if (audioSource != null && walkLoopSound != null)
                {
                    audioSource.pitch = Random.Range(0.85f, 1.15f); 
                    audioSource.PlayOneShot(walkLoopSound, 0.2f);   
                }
            }
        }
        else
        {
            moveGroanTimer = 0f; 
            
            if (rb != null) rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

            attackTimer += Time.deltaTime;
            if (attackTimer >= attackInterval)
            {
                AttackPlayer();
                attackTimer = 0f; 
            }
        }
    }

    void MoveTowardsPlayer()
    {
        if (stats == null) return;

        float dirX = Mathf.Sign(playerTransform.position.x - transform.position.x);

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(dirX * stats.moveSpeed, rb.linearVelocity.y);
        }

        Vector3 scale = transform.localScale;
        scale.x = (dirX > 0) ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    void AttackPlayer()
    {
        if (enemyAnimator != null) enemyAnimator.SetTrigger("Attack");

        if (audioSource != null && enemyAttackSound != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(enemyAttackSound, 0.4f); 
        }

        // --- HATA ÇÖZÜMÜ: SAHTE HASAR YERİNE GERÇEK STAT HASARI ---
        int finalDamage = (stats != null && stats.damage > 0) ? stats.damage : damageAmount;

        if (PlayerStats.Instance != null)
            PlayerStats.Instance.TakeDamage(finalDamage);
        else
        {
            PlayerStats pStats = playerTransform.GetComponent<PlayerStats>();
            if (pStats != null) pStats.TakeDamage(finalDamage);
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (audioSource != null) audioSource.Stop(); 
        if (enemyAnimator != null) enemyAnimator.SetTrigger("Die");

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        gameObject.layer = 0; 
        gameObject.tag = "Untagged"; 
        Destroy(gameObject, 1f); 
    }
}