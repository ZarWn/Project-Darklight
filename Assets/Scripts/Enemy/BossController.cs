using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    [Header("Boss Ayarları")]
    public float stopDistance = 1.5f;
    public float normalAttackInterval = 2f;
    public float phase2AttackInterval = 1.2f;
    public float phase3AttackInterval = 0.8f;

    [Header("Alan Saldırısı")]
    public float aoeRange = 3f;
    public int aoeDamage = 20;
    public float aoeInterval = 5f;

    [Header("Hızlanma")]
    public float normalSpeed = 1.5f;
    public float phase2Speed = 2.5f;
    public float phase3Speed = 3.5f;

    private Transform playerTransform;
    private EnemyStats stats;
    private float attackTimer = 0f;
    private float aoeTimer = 0f;
    private float attackInterval;
    private int currentPhase = 1;
    private LayerMask playerLayer;

    void Start()
    {
        stats = GetComponent<EnemyStats>();
        attackInterval = normalAttackInterval;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        playerLayer = LayerMask.GetMask("Player");

        // Boss UI'ı göster
        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null)
        {
            uiManager.ShowBossHP(stats.maxHP, GetBossName());
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        CheckPhase();
        HandleMovement();
        HandleAttack();
        HandleAOE();
        UpdateBossHP();
    }

    void CheckPhase()
    {
        float hpPercent = stats.GetHPPercent();

        if (hpPercent <= 0.3f && currentPhase != 3)
        {
            currentPhase = 3;
            stats.moveSpeed = phase3Speed;
            attackInterval = phase3AttackInterval;
            Debug.Log("BOSS FAZ 3! Çok tehlikeli!");
        }
        else if (hpPercent <= 0.6f && currentPhase == 1)
        {
            currentPhase = 2;
            stats.moveSpeed = phase2Speed;
            attackInterval = phase2AttackInterval;
            Debug.Log("BOSS FAZ 2! Hızlandı!");
        }
    }

    void HandleMovement()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer > stopDistance)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            float newX = transform.position.x + direction.x * stats.moveSpeed * Time.deltaTime;
            transform.position = new Vector3(newX, transform.position.y, 0f);

            if (direction.x > 0)
                transform.localScale = new Vector3(2, 2, 1);
            else
                transform.localScale = new Vector3(-2, 2, 1);
        }
    }

    void HandleAttack()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= stopDistance)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= attackInterval)
            {
                attackTimer = 0f;
                AttackPlayer();
            }
        }
    }

    void AttackPlayer()
    {
        PlayerStats playerStats = playerTransform.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.TakeDamage(stats.damage);
            Debug.Log($"Boss oyuncuya {stats.damage} hasar verdi! (Faz {currentPhase})");
        }
    }

    void HandleAOE()
    {
        if (currentPhase < 3) return;

        aoeTimer += Time.deltaTime;

        if (aoeTimer >= aoeInterval)
        {
            aoeTimer = 0f;
            AOEAttack();
        }
    }

    void AOEAttack()
    {
        Debug.Log("Boss Alan Saldırısı!");

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            aoeRange,
            LayerMask.GetMask("Player")
        );

        foreach (Collider2D hit in hits)
        {
            PlayerStats playerStats = hit.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(aoeDamage);
                Debug.Log($"Boss alan saldırısı! {aoeDamage} hasar verildi!");
            }
        }
    }

    void UpdateBossHP()
    {
        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null)
        {
            uiManager.UpdateBossHP(stats.currentHP);
        }
    }

    string GetBossName()
    {
    if (FloorManager.Instance != null)
    {
        int currentFloor = FloorManager.Instance.currentFloor;
        if (currentFloor <= 6) return "Zindan Bekçisi";
        if (currentFloor <= 9) return "Karanlık Şövalye";
        if (currentFloor <= 12) return "Kule Efendisi";
        return "Karanlık Kral";
    }
    return "BOSS";
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aoeRange);
    }
}