using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Saldırı Ayarları")]
    public float attackRange = 2f;
    public float attackCooldown = 0.5f;
    public int attackDamage = 10;
    public LayerMask enemyLayer;

    private float lastAttackTime;
    private float attackTimer;
    private int attackDirection = 1; // 1 = sağ, -1 = sol

    void Update()
    {
        AutoAttack();
    }

    void AutoAttack()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0f;

            // Önce sağa bak, sağda düşman var mı?
            bool hitRight = PerformAttack(1);

            // Sola bak, solda düşman var mı?
            bool hitLeft = PerformAttack(-1);
        }
    }

    bool PerformAttack(int direction)
    {
        Vector2 attackPoint = (Vector2)transform.position +
                              Vector2.right * direction * attackRange;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint,
            0.5f,
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

        return hitEnemies.Length > 0;
    }

    // Yetenek sistemi tarafından çağrılacak metodlar
    public void IncreaseAttackSpeed(float amount)
    {
        attackCooldown = Mathf.Max(0.1f, attackCooldown - amount);
        Debug.Log($"Saldırı hızı arttı! Yeni cooldown: {attackCooldown}");
    }

    public void IncreaseAttackDamage(int amount)
    {
        attackDamage += amount;
        Debug.Log($"Saldırı hasarı arttı! Yeni hasar: {attackDamage}");
    }

    public void IncreaseAttackRange(float amount)
    {
        attackRange += amount;
        Debug.Log($"Saldırı menzili arttı! Yeni menzil: {attackRange}");
    }

    public void IncreaseFireDamage(int amount)
    {
    // Her vuruşta ekstra ateş hasarı
    attackDamage += amount;
    Debug.Log($"Ateş hasarı eklendi! Yeni hasar: {attackDamage}");
    }

    public void ActivateSuperSpeed()
    {
    attackCooldown = Mathf.Max(0.1f, attackCooldown - 0.2f);
    Debug.Log($"Süper hız aktif! Yeni cooldown: {attackCooldown}");
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