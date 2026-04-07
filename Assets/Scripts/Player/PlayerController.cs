using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Saldırı Ayarları")]
    public float attackRange = 1f;
    public float attackCooldown = 0.5f;
    public int attackDamage = 10;
    public LayerMask enemyLayer;

    private float attackTimer;
    private int hitCount = 0;
    private WeaponData currentWeapon;
    private PlayerStats playerStats;

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        ApplyWeapon();
    }

    void ApplyWeapon()
    {
        if (WeaponManager.Instance == null) return;

        currentWeapon = WeaponManager.Instance.GetSelectedWeapon();
        if (currentWeapon == null) return;

        // Silah özelliklerini uygula
        attackDamage = currentWeapon.damage;
        attackCooldown = currentWeapon.attackSpeed;
        attackRange = currentWeapon.range;

        // Negatif özellikler
        if (currentWeapon.weaponType == WeaponType.RuhTirpani)
        {
            playerStats.maxHP -= currentWeapon.maxHPPenalty;
            playerStats.currentHP = playerStats.maxHP;
        }

        Debug.Log($"Silah uygulandı: {currentWeapon.weaponName}");
    }

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
            hitCount++;

            PerformAttack(1);
            PerformAttack(-1);

            // Khaos Asası kendine hasar
            if (currentWeapon != null && currentWeapon.weaponType == WeaponType.KhaosAsasi)
            {
                playerStats.TakeDamage(currentWeapon.selfDamage);
            }
        }
    }

    bool PerformAttack(int direction)
    {
    if (currentWeapon != null && currentWeapon.aoeAttack)
    {
        return PerformAOEAttack();
    }

    // Rün Yayı: düşmandan geçen ok (raycast kullan)
    if (currentWeapon != null && currentWeapon.piercingShot)
    {
        return PerformPiercingAttack(direction);
    }

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
            int finalDamage = CalculateDamage();
            enemyStats.TakeDamage(finalDamage);

            if (currentWeapon != null && currentWeapon.lifeSteal > 0)
            {
                int healAmount = Mathf.RoundToInt(finalDamage * currentWeapon.lifeSteal);
                playerStats.HealHP(healAmount);
            }

            if (currentWeapon != null && currentWeapon.applyBleed)
            {
                StartCoroutine(ApplyBleed(enemyStats));
            }

            Debug.Log($"Düşmana {finalDamage} hasar verildi!");
        }
    }

    return hitEnemies.Length > 0;
    }

    bool PerformPiercingAttack(int direction)
    {
    // Tüm menzil boyunca raycast at
    RaycastHit2D[] hits = Physics2D.RaycastAll(
        transform.position,
        Vector2.right * direction,
        attackRange,
        enemyLayer
    );

    foreach (RaycastHit2D hit in hits)
    {
        EnemyStats enemyStats = hit.collider.GetComponent<EnemyStats>();
        if (enemyStats != null)
        {
            int finalDamage = CalculateDamage();
            enemyStats.TakeDamage(finalDamage);
            Debug.Log($"Yay ile düşmana {finalDamage} hasar verildi!");
        }
    }

    return hits.Length > 0;
    }

    bool PerformAOEAttack()
    {
        // Khaos Asası: tüm menzildeki düşmanlara vurur
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            transform.position,
            attackRange,
            enemyLayer
        );

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(CalculateDamage());
            }
        }

        return hitEnemies.Length > 0;
    }

    int CalculateDamage()
    {
        int damage = attackDamage;

        // Kritik hasar (Gece Bıçağı)
        if (currentWeapon != null && currentWeapon.critEvery > 0)
        {
            if (hitCount % currentWeapon.critEvery == 0)
            {
                damage = Mathf.RoundToInt(damage * currentWeapon.critMultiplier);
                Debug.Log("KRİTİK HASAR!");
            }
        }

        return damage;
    }

    System.Collections.IEnumerator ApplyBleed(EnemyStats enemy)
    {
        float elapsed = 0f;
        while (elapsed < currentWeapon.bleedDuration && enemy != null)
        {
            yield return new WaitForSeconds(1f);
            elapsed += 1f;

            if (enemy != null)
            {
                enemy.TakeDamage(currentWeapon.bleedDamage);

                // Kendine kanama hasarı
                playerStats.TakeDamage(currentWeapon.bleedSelfDamage);
            }
        }
    }

    // Yetenek sistemi metodları
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
        Gizmos.DrawWireSphere(rightAttack, 0.5f);
        Gizmos.DrawWireSphere(leftAttack, 0.5f);
    }
}