using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Saldırı Ayarları")]
    public float attackRange = 2f;
    public float attackCooldown = 0.5f;
    public int attackDamage = 10;
    public LayerMask enemyLayer;

    private float lastAttackTime;
    private PlayerStats stats;
    private int attackDirection = 1;

    void Start()
    {
        stats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 touchPos = Input.mousePosition;

            if (touchPos.x > Screen.width / 2f)
            {
                attackDirection = 1;
            }
            else
            {
                attackDirection = -1;
            }

            TryAttack();
        }
    }

    void TryAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;
        lastAttackTime = Time.time;
        PerformAttack();
    }

    void PerformAttack()
    {
        Vector2 attackPoint = (Vector2)transform.position +
                              Vector2.right * attackDirection * attackRange;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint,
            1f,
            enemyLayer
        );

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(attackDamage);
                Debug.Log($"Düşmana {attackDamage} hasar verildi!");
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 rightAttack = (Vector2)transform.position + Vector2.right * attackRange;
        Vector2 leftAttack = (Vector2)transform.position + Vector2.left * attackRange;
        Gizmos.DrawWireSphere(rightAttack, 1f);
        Gizmos.DrawWireSphere(leftAttack, 1f);
    }
}