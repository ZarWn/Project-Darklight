using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [Header("Saldırı Ayarları")]
    public float attackRange = 1f;
    public float attackCooldown = 0.5f;
    public int attackDamage = 10;
    public LayerMask enemyLayer;

    private float attackTimer;
    private int hitCount = 0;
    private WeaponData currentWeapon;
    private PlayerStats playerStats;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        if (playerStats == null)
            playerStats = FindFirstObjectByType<PlayerStats>();

        enemyLayer = LayerMask.GetMask("Enemy");
        ApplyWeapon();
    }

    void OnEnable()
    {
        enemyLayer = LayerMask.GetMask("Enemy");
        playerStats = GetComponent<PlayerStats>();
        if (playerStats == null)
            playerStats = FindFirstObjectByType<PlayerStats>();
    }

    void ApplyWeapon()
    {
        enemyLayer = LayerMask.GetMask("Enemy");

        if (WeaponManager.Instance == null) return;

        currentWeapon = WeaponManager.Instance.GetSelectedWeapon();
        if (currentWeapon == null) return;

        attackDamage = currentWeapon.damage;
        attackCooldown = currentWeapon.attackSpeed;
        attackRange = currentWeapon.range;

        if (currentWeapon.weaponType == WeaponType.RuhTirpani)
        {
            if (playerStats != null)
            {
                playerStats.maxHP -= currentWeapon.maxHPPenalty;
                playerStats.currentHP = Mathf.Min(
                    playerStats.currentHP,
                    playerStats.maxHP
                );
            }
        }

        Debug.Log($"Silah uygulandi: {currentWeapon.weaponName}");
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

            // Khaos Asası kendine hasar - sadece düşman varsa
            if (currentWeapon != null &&
                currentWeapon.weaponType == WeaponType.KhaosAsasi)
            {
                // Etrafta düşman varsa kendine hasar ver
                Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(
                    transform.position, attackRange, enemyLayer
                );
                if (nearbyEnemies.Length > 0 && playerStats != null)
                    playerStats.TakeDamage(currentWeapon.selfDamage);
            }
        }
    }

    bool PerformAttack(int direction)
    {
        if (currentWeapon != null && currentWeapon.aoeAttack)
            return PerformAOEAttack();

        if (currentWeapon != null && currentWeapon.piercingShot)
            return PerformPiercingAttack(direction);

        Vector2 attackPoint = (Vector2)transform.position +
                              Vector2.right * direction * attackRange;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint, 0.5f, enemyLayer
        );

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                int finalDamage = CalculateDamage();
                enemyStats.TakeDamage(finalDamage);

                // Can çalma (Ruh Tırpanı)
                if (currentWeapon != null && currentWeapon.lifeSteal > 0)
                {
                    int healAmount = Mathf.RoundToInt(
                        finalDamage * currentWeapon.lifeSteal
                    );
                    if (playerStats != null)
                        playerStats.HealHP(healAmount);
                }

                // Kanama (Kan Mızrağı)
                 if (currentWeapon != null && currentWeapon.applyBleed && !enemyStats.isBleedingAlready)
                StartCoroutine(ApplyBleed(enemyStats));
            }
        }

        return hitEnemies.Length > 0;
    }

    bool PerformAOEAttack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            transform.position, attackRange, enemyLayer
        );

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
            if (enemyStats != null)
                enemyStats.TakeDamage(CalculateDamage());
        }

        return hitEnemies.Length > 0;
    }

    bool PerformPiercingAttack(int direction)
    {
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

    int CalculateDamage()
    {
        int damage = attackDamage;

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
    if (enemy == null) yield break;
    if (enemy.isDead) yield break;
    if (enemy.isBleedingAlready) yield break;

    enemy.isBleedingAlready = true;
    float elapsed = 0f;

    while (elapsed < currentWeapon.bleedDuration)
    {
        yield return new WaitForSeconds(1f);
        elapsed += 1f;

        // Düşman öldüyse dur
        if (enemy == null || enemy.isDead)
        {
            yield break;
        }

        enemy.TakeDamage(currentWeapon.bleedDamage);

        // Kendine çok az hasar ver
        if (playerStats != null)
            playerStats.TakeDamage(currentWeapon.bleedSelfDamage);
    }

    if (enemy != null)
        enemy.isBleedingAlready = false;
}

    public void IncreaseAttackSpeed(float amount)
    {
        attackCooldown = Mathf.Max(0.1f, attackCooldown - amount);
        Debug.Log($"Saldiri hizi artti! Yeni cooldown: {attackCooldown}");
    }

    public void IncreaseAttackDamage(int amount)
    {
        attackDamage += amount;
        Debug.Log($"Saldiri hasari artti! Yeni hasar: {attackDamage}");
    }

    public void IncreaseAttackRange(float amount)
    {
        attackRange += amount;
        Debug.Log($"Saldiri menzili artti! Yeni menzil: {attackRange}");
    }

    public void IncreaseFireDamage(int amount)
    {
        attackDamage += amount;
        Debug.Log($"Ates hasari eklendi! Yeni hasar: {attackDamage}");
    }

    public void ActivateSuperSpeed()
    {
        attackCooldown = Mathf.Max(0.1f, attackCooldown - 0.2f);
        Debug.Log($"Super hiz aktif! Yeni cooldown: {attackCooldown}");
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