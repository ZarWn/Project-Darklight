using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // SAHNE GEÇİŞLERİNİ ANLAMAK İÇİN EKLENDİ

public class ActiveAbilityManager : MonoBehaviour
{
    public static ActiveAbilityManager Instance;

    [Header("Yetenek Slotları")]
    private ActiveAbility[] abilities = new ActiveAbility[4];
    private float[] cooldownTimers = new float[4];
    private bool[] isOnCooldown = new bool[4];

    private PlayerStats playerStats;
    private PlayerController playerController;

    public delegate void OnAbilityCast(int slot);
    public static event OnAbilityCast onAbilityCast;

    public delegate void OnCooldownChanged(int slot, float remaining, float total);
    public static event OnCooldownChanged onCooldownChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        transform.SetParent(null); 
        DontDestroyOnLoad(gameObject);
        
        InitializeAbilities();
    }

    // YENİ: Sahne (Stage) değişimlerini dinlemeye başla
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // YENİ: Yeni sahne yüklendiğinde otomatik çalışır
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Yeni bölümdeki oyuncuyu anında bul
        playerStats = FindFirstObjectByType<PlayerStats>();
        playerController = FindFirstObjectByType<PlayerController>();

        // Tüm yetenek sürelerini sıfırla
        ResetAllCooldowns();
    }

    // YENİ: Tüm bekleme sürelerini sıfırlayan ve UI'a haber veren fonksiyon
    public void ResetAllCooldowns()
    {
        for (int i = 0; i < 4; i++)
        {
            cooldownTimers[i] = 0f;
            isOnCooldown[i] = false;
            
            // Eğer arayüzde (UI) dönen bir bekleme barı varsa onu da anında boşaltır
            if (abilities[i] != null)
            {
                onCooldownChanged?.Invoke(i, 0f, abilities[i].cooldown);
            }
        }
        Debug.Log("Yeni Stage: Tüm yetenek bekleme süreleri sıfırlandı!");
    }

    void Update()
    {
        // Her ihtimale karşı oyuncu sonradan spawn olursa diye güvenlik kontrolü
        if (playerStats == null) playerStats = FindFirstObjectByType<PlayerStats>();
        if (playerController == null) playerController = FindFirstObjectByType<PlayerController>();

        UpdateCooldowns();
    }

    void InitializeAbilities()
    {
        abilities[0] = new ActiveAbility { name = "Göksel Çarpma", cooldown = 15f };
        abilities[1] = new ActiveAbility { name = "Mutlak Kalkan", cooldown = 20f };
        abilities[2] = new ActiveAbility { name = "Savaş Çığlığı", cooldown = 25f };
        abilities[3] = new ActiveAbility { name = "Kanlı Girdap", cooldown = 12f };

        for (int i = 0; i < 4; i++)
        {
            cooldownTimers[i] = 0f;
            isOnCooldown[i] = false;
        }
    }

    void UpdateCooldowns()
    {
        for (int i = 0; i < 4; i++)
        {
            if (isOnCooldown[i])
            {
                cooldownTimers[i] -= Time.deltaTime;
                
                float remaining = Mathf.Max(0, cooldownTimers[i]);
                onCooldownChanged?.Invoke(i, remaining, abilities[i].cooldown);

                if (cooldownTimers[i] <= 0)
                {
                    isOnCooldown[i] = false;
                    cooldownTimers[i] = 0;
                }
            }
        }
    }

    public void CastAbility(int slot)
    {
        if (slot < 0 || slot >= 4) return;
        if (abilities[slot] == null) return;

        if (isOnCooldown[slot])
        {
            Debug.LogWarning($"[{abilities[slot].name}] henüz hazır değil! Kalan süre: {Mathf.CeilToInt(cooldownTimers[slot])} saniye.");
            return;
        }

        ExecuteAbility(slot);

        cooldownTimers[slot] = abilities[slot].cooldown;
        isOnCooldown[slot] = true;

        onAbilityCast?.Invoke(slot);
        Debug.Log($"---> YETENEK KULLANILDI: {abilities[slot].name}");
    }

    void ExecuteAbility(int slot)
    {
        switch (slot)
        {
            case 0: AbilityCelestialStrike(); break;
            case 1: AbilityAbsoluteShield(); break;
            case 2: AbilityBattleCry(); break;
            case 3: AbilityBloodVortex(); break;
        }
    }

    void AbilityCelestialStrike()
    {
        if (playerController == null) return;
        LayerMask enemyLayer = LayerMask.GetMask("Enemy");
        Collider2D[] enemies = Physics2D.OverlapCircleAll(playerController.transform.position, 15f, enemyLayer);
        foreach (Collider2D enemy in enemies)
        {
            EnemyStats stats = enemy.GetComponent<EnemyStats>();
            if (stats != null) stats.TakeDamage(100);
        }
    }

    void AbilityAbsoluteShield()
    {
        if (playerStats != null) StartCoroutine(ShieldCoroutine());
    }

    IEnumerator ShieldCoroutine()
    {
        playerStats.isInvincible = true;
        yield return new WaitForSeconds(4f);
        if (playerStats != null) playerStats.isInvincible = false;
    }

    void AbilityBattleCry()
    {
        if (playerController != null) StartCoroutine(BattleCryCoroutine());
    }

    IEnumerator BattleCryCoroutine()
    {
        float originalCooldown = playerController.attackCooldown;
        playerController.attackCooldown = originalCooldown / 2f; 
        playerController.attackDamage += 20; 
        
        yield return new WaitForSeconds(5f);
        
        if (playerController != null) {
            playerController.attackCooldown = originalCooldown;
            playerController.attackDamage -= 20;
        }
    }

    void AbilityBloodVortex()
    {
        if (playerController == null) return;
        LayerMask enemyLayer = LayerMask.GetMask("Enemy");
        Collider2D[] enemies = Physics2D.OverlapCircleAll(playerController.transform.position, 5f, enemyLayer);
        bool hitSomeone = false;
        foreach (Collider2D enemy in enemies)
        {
            EnemyStats stats = enemy.GetComponent<EnemyStats>();
            if (stats != null)
            {
                stats.TakeDamage(40);
                hitSomeone = true;
            }
        }
        if (hitSomeone && playerStats != null) playerStats.HealHP(30);
    }
}

public class ActiveAbility
{
    public string name;
    public float cooldown;
}