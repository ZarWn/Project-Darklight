using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [Header("Saldırı Ayarları")]
    public float attackRange = 1f;
    public float attackCooldown = 0.5f;
    public int attackDamage = 10;
    
    [Tooltip("Animasyon başladıktan kaç saniye sonra hasar verilsin?")]
    public float hitDelay = 0.2f; 
    
    public LayerMask enemyLayer;

    // --- YENİ EKLENEN: KILIÇ SESİ ---
    [Header("Ses Ayarları")]
    public AudioClip swordAttackSound;
    private AudioSource audioSource;
    // --------------------------------

    private Animator animator;
    private float attackTimer;
    private int hitCount = 0;
    private WeaponData currentWeapon;
    private PlayerStats playerStats;

    private float baseAnimSpeed = 1f;

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
        animator = GetComponent<Animator>();
        
        // --- HOPARLÖRÜ KODA TANIT ---
        audioSource = GetComponent<AudioSource>();
        // ----------------------------
        
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
                playerStats.currentHP = Mathf.Min(playerStats.currentHP, playerStats.maxHP);
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
            if (CheckIfEnemyNearby())
            {
                attackTimer = 0f;
                hitCount++;

                FaceNearestEnemy();

                if (animator != null)
                {
                    float targetAnimSpeed = 0.5f / attackCooldown; 
                    animator.speed = targetAnimSpeed; 

                    animator.ResetTrigger("Attack");
                    float randomAttackIndex = Random.Range(0f, 1f);
                    animator.SetFloat("AttackIndex", randomAttackIndex);
                    animator.SetTrigger("Attack");
                }

                // --- YENİ EKLENEN: KILIÇ SESİNİ ÇAL ---
                if (audioSource != null && swordAttackSound != null)
                {
                    // Peş peşe vurmalarda monotonluğu bozmak için sesi hafif inceltip kalınlaştır
                    audioSource.pitch = Random.Range(0.9f, 1.15f);
                    // Kılıç sesini %60 volümde çal (Eğer kafa ütülerse 0.4f'e falan düşürebilirsin)
                    audioSource.PlayOneShot(swordAttackSound, 0.6f); 
                }
                // --------------------------------------

                StartCoroutine(DelayedHitSequence());
            }
            else
            {
                attackTimer = attackCooldown;

                if (animator != null)
                {
                    animator.ResetTrigger("Attack");
                    animator.speed = baseAnimSpeed; 
                }
            }
        }
    }

    void FaceNearestEnemy()
    {
        int facingDirection = 1; 
        
        Vector2 rightPoint = (Vector2)transform.position + Vector2.right * attackRange;
        if (Physics2D.OverlapCircle(rightPoint, 0.5f, enemyLayer) != null)
        {
            facingDirection = 1;
        }
        else 
        {
            Vector2 leftPoint = (Vector2)transform.position + Vector2.left * attackRange;
            if (Physics2D.OverlapCircle(leftPoint, 0.5f, enemyLayer) != null)
            {
                facingDirection = -1;
            }
        }
        
        if (currentWeapon != null && currentWeapon.piercingShot)
        {
             if (Physics2D.Raycast(transform.position, Vector2.right, attackRange, enemyLayer)) 
                 facingDirection = 1;
             else if (Physics2D.Raycast(transform.position, Vector2.left, attackRange, enemyLayer)) 
                 facingDirection = -1;
        }

        if (currentWeapon != null && currentWeapon.aoeAttack)
        {
             Collider2D enemy = Physics2D.OverlapCircle(transform.position, attackRange, enemyLayer);
             if (enemy != null)
             {
                  facingDirection = (enemy.transform.position.x >= transform.position.x) ? 1 : -1;
             }
        }

        Vector3 currentScale = transform.localScale;
        currentScale.x = Mathf.Abs(currentScale.x) * facingDirection;
        transform.localScale = currentScale;
    }

    System.Collections.IEnumerator DelayedHitSequence()
    {
        yield return new WaitForSeconds(hitDelay);

        PerformAttack(1);
        PerformAttack(-1);

        if (currentWeapon != null && currentWeapon.weaponType == WeaponType.KhaosAsasi)
        {
            Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(
                transform.position, attackRange, enemyLayer
            );
            if (nearbyEnemies.Length > 0 && playerStats != null)
                playerStats.TakeDamage(currentWeapon.selfDamage);
        }
        
        yield return new WaitForSeconds(attackCooldown - hitDelay);
        if (animator != null && !CheckIfEnemyNearby()) 
        {
            animator.speed = baseAnimSpeed;
        }
    }

    bool CheckIfEnemyNearby()
    {
        if (currentWeapon != null && currentWeapon.aoeAttack)
        {
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
            return enemies.Length > 0;
        }

        Vector2 rightPoint = (Vector2)transform.position + Vector2.right * attackRange;
        Vector2 leftPoint = (Vector2)transform.position + Vector2.left * attackRange;

        Collider2D[] rightEnemies = Physics2D.OverlapCircleAll(rightPoint, 0.5f, enemyLayer);
        Collider2D[] leftEnemies = Physics2D.OverlapCircleAll(leftPoint, 0.5f, enemyLayer);

        return rightEnemies.Length > 0 || leftEnemies.Length > 0 || CheckPiercingEnemies();
    }

    bool CheckPiercingEnemies()
    {
        if (currentWeapon != null && currentWeapon.piercingShot)
        {
            RaycastHit2D[] rightHits = Physics2D.RaycastAll(transform.position, Vector2.right, attackRange, enemyLayer);
            RaycastHit2D[] leftHits = Physics2D.RaycastAll(transform.position, Vector2.left, attackRange, enemyLayer);
            return rightHits.Length > 0 || leftHits.Length > 0;
        }
        return false;
    }

    bool PerformAttack(int direction)
    {
        if (currentWeapon != null && currentWeapon.aoeAttack)
            return PerformAOEAttack();

        if (currentWeapon != null && currentWeapon.piercingShot)
            return PerformPiercingAttack(direction);

        Vector2 attackPoint = (Vector2)transform.position + Vector2.right * direction * attackRange;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint, 0.5f, enemyLayer);

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
                    if (playerStats != null)
                        playerStats.HealHP(healAmount);
                }

                if (currentWeapon != null && currentWeapon.applyBleed && !enemyStats.isBleedingAlready)
                    StartCoroutine(ApplyBleed(enemyStats));
            }
        }

        return hitEnemies.Length > 0;
    }

    bool PerformAOEAttack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);

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
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.right * direction, attackRange, enemyLayer);

        foreach (RaycastHit2D hit in hits)
        {
            EnemyStats enemyStats = hit.collider.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                int finalDamage = CalculateDamage();
                enemyStats.TakeDamage(finalDamage);
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
            }
        }
        return damage;
    }

    System.Collections.IEnumerator ApplyBleed(EnemyStats enemy)
    {
        if (enemy == null || enemy.isDead || enemy.isBleedingAlready) yield break;

        enemy.isBleedingAlready = true;
        float elapsed = 0f;

        while (elapsed < currentWeapon.bleedDuration)
        {
            yield return new WaitForSeconds(1f);
            elapsed += 1f;

            if (enemy == null || enemy.isDead) yield break;

            enemy.TakeDamage(currentWeapon.bleedDamage);
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