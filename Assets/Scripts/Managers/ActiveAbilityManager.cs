using UnityEngine;
using System.Collections;

public class ActiveAbilityManager : MonoBehaviour
{
    public static ActiveAbilityManager Instance;

    [Header("Enerji Sistemi")]
    public float maxEnergy = 100f;
    public float currentEnergy = 100f;
    public float energyRegenRate = 10f; // saniyede 10 enerji

    [Header("Yetenek Slotları")]
    private ActiveAbility[] abilities = new ActiveAbility[4];
    private float[] cooldownTimers = new float[4];
    private bool[] isOnCooldown = new bool[4];

    private PlayerStats playerStats;
    private PlayerController playerController;

    // Events
    public delegate void OnAbilityCast(int slot);
    public static event OnAbilityCast onAbilityCast;

    public delegate void OnEnergyChanged(float current, float max);
    public static event OnEnergyChanged onEnergyChanged;

    public delegate void OnCooldownChanged(int slot, float remaining, float total);
    public static event OnCooldownChanged onCooldownChanged;

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

        InitializeAbilities();
    }

    void Start()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
        playerController = FindFirstObjectByType<PlayerController>();
        currentEnergy = maxEnergy;
    }

    void Update()
    {
        RegenerateEnergy();
        UpdateCooldowns();
    }

    void InitializeAbilities()
    {
        // Slot 0: Harita Taraması (Map Scan)
        abilities[0] = new ActiveAbility
        {
            name = "Harita Taraması",
            energyCost = 30,
            cooldown = 15f,
            description = "Ekrandaki tüm düşmanlara hasar ver"
        };

        // Slot 1: Kalkan Bariyeri (Shield Barrier)
        abilities[1] = new ActiveAbility
        {
            name = "Kalkan Bariyeri",
            energyCost = 40,
            cooldown = 20f,
            description = "3 saniye boyunca hasar almaz"
        };

        // Slot 2: Frenzy Atağı (Frenzy Attack)
        abilities[2] = new ActiveAbility
        {
            name = "Frenzy Atağı",
            energyCost = 50,
            cooldown = 25f,
            description = "5 saniye x2 saldırı hızı"
        };

        // Slot 3: Teleport
        abilities[3] = new ActiveAbility
        {
            name = "Teleport",
            energyCost = 25,
            cooldown = 12f,
            description = "3 birim ileriye ışınla"
        };

        // Cooldown'ları sıfırla
        for (int i = 0; i < 4; i++)
        {
            cooldownTimers[i] = 0f;
            isOnCooldown[i] = false;
        }
    }

    void RegenerateEnergy()
    {
        if (currentEnergy < maxEnergy)
        {
            currentEnergy += energyRegenRate * Time.deltaTime;
            currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
            onEnergyChanged?.Invoke(currentEnergy, maxEnergy);
        }
    }

    void UpdateCooldowns()
    {
        for (int i = 0; i < 4; i++)
        {
            if (isOnCooldown[i])
            {
                cooldownTimers[i] -= Time.deltaTime;
                
                // Cooldown event trigger
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

    public bool CastAbility(int slot)
    {
        if (slot < 0 || slot >= 4)
            return false;

        if (abilities[slot] == null)
        {
            Debug.LogWarning($"Slot {slot} boş!");
            return false;
        }

        // Cooldown kontrolü
        if (isOnCooldown[slot])
        {
            Debug.Log($"{abilities[slot].name} hala cooldown'da!");
            return false;
        }

        // Enerji kontrolü
        if (currentEnergy < abilities[slot].energyCost)
        {
            Debug.Log($"Yeterli enerji yok! Gerekli: {abilities[slot].energyCost}, Mevcut: {currentEnergy}");
            return false;
        }

        // Yeteneği cast et
        ExecuteAbility(slot);

        // Enerji harca
        currentEnergy -= abilities[slot].energyCost;
        currentEnergy = Mathf.Max(0, currentEnergy);
        onEnergyChanged?.Invoke(currentEnergy, maxEnergy);

        // Cooldown başlat
        cooldownTimers[slot] = abilities[slot].cooldown;
        isOnCooldown[slot] = true;

        // Event trigger
        onAbilityCast?.Invoke(slot);

        Debug.Log($"Yetenek cast: {abilities[slot].name}");
        return true;
    }

    void ExecuteAbility(int slot)
    {
        switch (slot)
        {
            case 0: // Harita Taraması
                AbilityMapScan();
                break;
            case 1: // Kalkan Bariyeri
                AbilityShieldBarrier();
                break;
            case 2: // Frenzy Atağı
                AbilityFrenzyAttack();
                break;
            case 3: // Teleport
                AbilityTeleport();
                break;
        }
    }

    void AbilityMapScan()
    {
        // Ekrandaki tüm düşmanlara hasar
        LayerMask enemyLayer = LayerMask.GetMask("Enemy");
        Collider2D[] enemies = Physics2D.OverlapCircleAll(
            playerController.transform.position, 
            20f, 
            enemyLayer
        );

        int damagePerEnemy = 25;
        foreach (Collider2D enemy in enemies)
        {
            EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(damagePerEnemy);
            }
        }

        Debug.Log($"Map Scan: {enemies.Length} düşman vuruldu!");
    }

    void AbilityShieldBarrier()
    {
        // 3 saniye invincible
        if (playerStats != null)
        {
            StartCoroutine(ShieldBarrierCoroutine());
        }
    }

    IEnumerator ShieldBarrierCoroutine()
    {
        playerStats.isInvincible = true;
        yield return new WaitForSeconds(3f);
        playerStats.isInvincible = false;
        Debug.Log("Shield Barrier bitti!");
    }

    void AbilityFrenzyAttack()
    {
        // 5 saniye x2 attack speed
        if (playerController != null)
        {
            StartCoroutine(FrenzyAttackCoroutine());
        }
    }

    IEnumerator FrenzyAttackCoroutine()
    {
        float originalCooldown = playerController.attackCooldown;
        playerController.attackCooldown = originalCooldown / 2f; // x2 speed
        yield return new WaitForSeconds(5f);
        playerController.attackCooldown = originalCooldown;
        Debug.Log("Frenzy Attack bitti!");
    }

    void AbilityTeleport()
    {
        // 3 birim ileriye teleport
        Vector3 teleportDistance = playerController.transform.right * 3f;
        playerController.transform.position += teleportDistance;
        Debug.Log("Teleported!");
    }

    // Getter metodları
    public ActiveAbility GetAbility(int slot)
    {
        if (slot >= 0 && slot < 4)
            return abilities[slot];
        return null;
    }

    public bool IsAbilityOnCooldown(int slot)
    {
        if (slot >= 0 && slot < 4)
            return isOnCooldown[slot];
        return true;
    }

    public float GetCooldownRemaining(int slot)
    {
        if (slot >= 0 && slot < 4)
            return Mathf.Max(0, cooldownTimers[slot]);
        return 0;
    }

    public float GetEnergyPercent()
    {
        return currentEnergy / maxEnergy;
    }

    public bool CanCastAbility(int slot)
    {
        if (slot < 0 || slot >= 4) return false;
        if (abilities[slot] == null) return false;
        if (isOnCooldown[slot]) return false;
        if (currentEnergy < abilities[slot].energyCost) return false;
        return true;
    }
}

// Yetenek data class
public class ActiveAbility
{
    public string name;
    public float energyCost;
    public float cooldown;
    public string description;
}
