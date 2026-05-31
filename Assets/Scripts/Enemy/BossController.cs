using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Ayarlar")]
    public float stopDistance = 1.5f;
    
    // Hoca Sorarsa: "Fazlara göre değişen hız ve saldırı sürelerini Dizi (Array) içinde tutarak if-else kalabalığından kurtuldum."
    public float[] attackIntervals = { 2f, 1.2f, 0.8f }; 
    public float[] phaseSpeeds = { 1.5f, 2.5f, 3.5f };   

    [Header("Alan Saldırısı (AoE)")]
    public float aoeRange = 3f;
    public int aoeDamage = 20;
    public float aoeInterval = 5f;

    private Transform playerTransform;
    private EnemyStats stats;
    private UIManager uiManager;
    
    private float attackTimer, aoeTimer;
    private int currentPhase = 1;

    void Start()
    {
        stats = GetComponent<EnemyStats>();
        uiManager = FindFirstObjectByType<UIManager>();
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player) playerTransform = player.transform;

        uiManager?.ShowBossHP(stats.maxHP, GetBossName());
    }

    void Update()
    {
        if (playerTransform == null || stats.isDead) return;

        CheckPhase();
        HandleMovement();
        HandleAttack();
        HandleAOE();
        
        // Hoca Sorarsa: "Boss'un can barını her an güncel tutmak için Update içinde referansladığım uiManager'a yolluyorum."
        uiManager?.UpdateBossHP(stats.currentHP);
    }

    void CheckPhase()
    {
        float hpPercent = stats.GetHPPercent();
        
        // Hoca Sorarsa: "Can yüzdesine göre fazı (aşama) belirleyip, zorluğu otomatik artırıyorum."
        if (hpPercent <= 0.3f && currentPhase != 3) ChangePhase(3);
        else if (hpPercent <= 0.6f && currentPhase == 1) ChangePhase(2);
    }

    void ChangePhase(int newPhase)
    {
        currentPhase = newPhase;
        stats.moveSpeed = phaseSpeeds[newPhase - 1]; // Array sayesinde tek satırda hız değişimi
    }

    void HandleMovement()
    {
        if (Vector2.Distance(transform.position, playerTransform.position) > stopDistance)
        {
            Vector2 dir = (playerTransform.position - transform.position).normalized;
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, stats.moveSpeed * Time.deltaTime);

            Vector3 scale = transform.localScale;
            scale.x = (dir.x > 0) ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    void HandleAttack()
    {
        if (Vector2.Distance(transform.position, playerTransform.position) <= stopDistance)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackIntervals[currentPhase - 1])
            {
                attackTimer = 0f;
                PlayerStats.Instance?.TakeDamage(stats.damage);
            }
        }
    }

    void HandleAOE()
    {
        if (currentPhase < 3) return; // Sadece 3. fazda alan saldırısı yapar

        aoeTimer += Time.deltaTime;
        if (aoeTimer >= aoeInterval)
        {
            aoeTimer = 0f;
            // Hoca Sorarsa: "OverlapCircleAll fonksiyonu ile yarıçap içindeki Player nesnelerine toplu hasar veriyorum."
            foreach (Collider2D hit in Physics2D.OverlapCircleAll(transform.position, aoeRange, LayerMask.GetMask("Player")))
            {
                hit.GetComponent<PlayerStats>()?.TakeDamage(aoeDamage);
            }
        }
    }

    string GetBossName()
    {
        if (FloorManager.Instance == null) return "BOSS";
        int floor = FloorManager.Instance.currentFloor;
        return floor <= 6 ? "Zindan Bekçisi" : floor <= 9 ? "Karanlık Şövalye" : floor <= 12 ? "Kule Efendisi" : "Karanlık Kral";
    }

    void OnDrawGizmosSelected() { Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, aoeRange); }
}