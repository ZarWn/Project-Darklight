using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Transform playerTransform;
    private EnemyStats stats;
    private Animator enemyAnimator; 

    [Header("Hareket Ayarları")]
    public float stopDistance = 1.5f;   

    [Header("Saldırı Ayarları")]
    public float attackInterval = 1.5f; 
    private float attackTimer = 0f;
    public int damageAmount = 5;        

    [HideInInspector] 
    public bool isDead = false;         

    [Header("Ses Ayarları")]
    public AudioClip walkLoopSound; // Artık yaklaşırken çıkacak yürüme/hırıltı sesi
    public AudioClip enemyAttackSound; 
    private AudioSource audioSource;   

    // Sadece yürürken çalışacak ses zamanlayıcıları
    private float moveGroanTimer = 0f;
    private float nextMoveGroanTime = 0f;

    void Start()
    {
        stats = GetComponent<EnemyStats>();
        enemyAnimator = GetComponent<Animator>(); 
        audioSource = GetComponent<AudioSource>(); 

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        attackTimer = attackInterval; 
        
        // Karakter doğduğunda ilk yürüme sesini 1-3 saniye arasında hızlıca versin
        nextMoveGroanTime = Random.Range(1f, 3f);
    }

    void Update()
    {
        if (isDead || playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        // EĞER OYUNCUYA UZAKSA VE YÜRÜYORSA
        if (distanceToPlayer > stopDistance)
        {
            MoveTowardsPlayer();
            attackTimer = attackInterval; 

            // --- YENİ: SADECE YAKLAŞIRKEN ÇIKAN DENGELİ SES ---
            moveGroanTimer += Time.deltaTime;
            if (moveGroanTimer >= nextMoveGroanTime)
            {
                moveGroanTimer = 0f;
                // Bir sonraki sesi 3 ile 7 saniye arası bekle (çok sık olmaması için)
                nextMoveGroanTime = Random.Range(3f, 7f); 

                if (audioSource != null && walkLoopSound != null)
                {
                    audioSource.pitch = Random.Range(0.85f, 1.15f); // Farklı zombi ses tonları
                    audioSource.PlayOneShot(walkLoopSound, 0.2f);   // Sesi %20 seviyesinde kısık çal
                }
            }
            // --------------------------------------------------
        }
        // EĞER MENZİLE GİRDİYSE VE DURUYORSA/SALDIRIYORSA
        else
        {
            moveGroanTimer = 0f; // Durduğu için yürüme sesi sayacını iptal et

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

        Vector2 currentPos = transform.position;
        Vector2 targetPos = playerTransform.position;
        Vector2 direction = (targetPos - currentPos).normalized;

        float newX = currentPos.x + direction.x * stats.moveSpeed * Time.deltaTime;
        transform.position = new Vector3(newX, currentPos.y, 0f);

        float currentScaleX = Mathf.Abs(transform.localScale.x); 
        float currentScaleY = transform.localScale.y;
        float currentScaleZ = transform.localScale.z;

        if (direction.x > 0)
            transform.localScale = new Vector3(currentScaleX, currentScaleY, currentScaleZ);
        else if (direction.x < 0)
            transform.localScale = new Vector3(-currentScaleX, currentScaleY, currentScaleZ);
    }

    void AttackPlayer()
    {
        if (enemyAnimator != null)
        {
            enemyAnimator.SetTrigger("Attack");
        }

        if (audioSource != null && enemyAttackSound != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(enemyAttackSound, 0.4f); // Saldırı sesini %40 seviyesinde çal
        }

        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.TakeDamage(damageAmount);
        }
        else
        {
            PlayerStats pStats = playerTransform.GetComponent<PlayerStats>();
            if (pStats != null)
            {
                pStats.TakeDamage(damageAmount);
            }
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (audioSource != null)
        {
            audioSource.Stop(); 
        }

        if (enemyAnimator != null)
        {
            enemyAnimator.SetTrigger("Die");
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
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