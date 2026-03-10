using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Transform playerTransform;
    private EnemyStats stats;
    private float attackTimer = 0f;
    public float attackInterval = 1.5f;
    public float stopDistance = 1.2f;

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

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

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
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        transform.position += (Vector3)(direction * stats.moveSpeed * Time.deltaTime);

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