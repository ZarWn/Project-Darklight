using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Transform playerTransform;
    private EnemyStats stats;
    private Animator enemyAnimator; 
    private Rigidbody2D rb; // FİZİK MOTORU EKLENDİ

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
        rb = GetComponent<Rigidbody2D>(); // Rigidbody Tanımlandı

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
            
            // --- HATA ÇÖZÜMÜ: OYUNCUYA YAKLAŞINCA FREN YAP ---
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

        // Sadece X (Sağ-Sol) ekseninde yönü bulur (-1 veya 1)
        float dirX = Mathf.Sign(playerTransform.position.x - transform.position.x);

        // --- HATA ÇÖZÜMÜ 2: UÇAN DÜŞMANLAR ---
        // Hoca Sorarsa: "Düşmanları 'transform.position' ile itmek, zemin çarpışmalarıyla birleşince havaya fırlamalarına neden oluyordu. Y eksenini (Aşağı düşüşü) yerçekimine bırakıp sadece X ekseninde Rigidbody.linearVelocity ile güç uygulayarak sorunu kökünden çözdüm."
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(dirX * stats.moveSpeed, rb.linearVelocity.y);
        }

        // Yönüne göre çevir (Flip)
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

        if (PlayerStats.Instance != null)
            PlayerStats.Instance.TakeDamage(damageAmount);
        else
        {
            PlayerStats pStats = playerTransform.GetComponent<PlayerStats>();
            if (pStats != null) pStats.TakeDamage(damageAmount);
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