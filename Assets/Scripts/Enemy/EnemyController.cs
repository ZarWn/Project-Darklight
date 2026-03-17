using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Transform playerTransform;
    private EnemyStats stats;
    private float attackTimer = 0f;
    public float attackInterval = 1.5f;
    public float stopDistance = 1.5f;

    void Start()
    {
        stats = GetComponent<EnemyStats>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(
            transform.position,
            playerTransform.position
        );

        if (distanceToPlayer > stopDistance)
        {
            MoveTowardsPlayer();
        }
        else
        {
            AttackPlayer();
        }
    }

    void MoveTowardsPlayer()
    {
        Vector2 currentPos = transform.position;
        Vector2 targetPos = playerTransform.position;
        Vector2 direction = (targetPos - currentPos).normalized;

        // Sadece X ekseninde hareket et
        float newX = currentPos.x + direction.x * stats.moveSpeed * Time.deltaTime;
        transform.position = new Vector3(newX, currentPos.y, 0f);

        // Yüz yönü
        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);
    }

    void AttackPlayer()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackInterval)
        {
            attackTimer = 0f;
            PlayerStats playerStats = playerTransform.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(stats.damage);
                Debug.Log($"Düşman oyuncuya {stats.damage} hasar verdi!");
            }
        }
    }
}