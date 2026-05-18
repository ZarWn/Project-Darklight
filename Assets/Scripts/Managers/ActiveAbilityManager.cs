using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; 

public class ActiveAbilityManager : MonoBehaviour
{
    public static ActiveAbilityManager Instance;

    [Header("Yetenek Slotları")]
    private ActiveAbility[] abilities = new ActiveAbility[3];
    private float[] cooldownTimers = new float[3];
    private bool[] isOnCooldown = new bool[3];

    [Header("Yetenek Efektleri (VFX)")]
    public GameObject celestialStrikeVFX; 
    public GameObject absoluteShieldVFX;  
    public GameObject battleCryVFX;       

    [Header("Yetenek Ses Efektleri (SFX)")]
    public AudioClip celestialStrikeSFX;  
    public AudioClip absoluteShieldSFX;   
    public AudioClip battleCrySFX;        
    private AudioSource audioSource;       

    [Header("Göksel Çarpma (Çoklu Yıldırım) Ayarları")]
    public int celestialStrikeCount = 4;       
    public float celestialStrikeRange = 8f;     
    public float timeBetweenStrikes = 0.25f;    
    public float strikeDamageRadius = 2.5f;     
    
    // --- YENİ: YILDIRIMIN YÜKSEKLİK VE BOYUT AYARLARI ---
    public float celestialStrikeYOffset = 1f;   // Yıldırımı aşağı/yukarı kaydırmak için (Örn: -2f yaparsan yere çöker)
    public float celestialStrikeScale = 5f;     // Yıldırımın devasalık boyutu (Örn: 4 veya 5 yapabilirsin)

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
        playerController = FindFirstObjectByType<PlayerController>();

        ResetAllCooldowns();
    }

    public void ResetAllCooldowns()
    {
        for (int i = 0; i < 3; i++)
        {
            cooldownTimers[i] = 0f;
            isOnCooldown[i] = false;
            
            if (abilities[i] != null)
            {
                onCooldownChanged?.Invoke(i, 0f, abilities[i].cooldown);
            }
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (playerStats == null) playerStats = FindFirstObjectByType<PlayerStats>();
        if (playerController == null) playerController = FindFirstObjectByType<PlayerController>();

        UpdateCooldowns();
    }

    void InitializeAbilities()
    {
        abilities[0] = new ActiveAbility { name = "Göksel Çarpma", cooldown = 15f };
        abilities[1] = new ActiveAbility { name = "Mutlak Kalkan", cooldown = 20f };
        abilities[2] = new ActiveAbility { name = "Savaş Çığlığı", cooldown = 25f };

        for (int i = 0; i < 3; i++)
        {
            cooldownTimers[i] = 0f;
            isOnCooldown[i] = false;
        }
    }

    void UpdateCooldowns()
    {
        for (int i = 0; i < 3; i++)
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
        if (slot < 0 || slot >= 3) return; 
        if (abilities[slot] == null) return;

        if (isOnCooldown[slot]) return;

        ExecuteAbility(slot);

        cooldownTimers[slot] = abilities[slot].cooldown;
        isOnCooldown[slot] = true;

        onAbilityCast?.Invoke(slot);
    }

    void ExecuteAbility(int slot)
    {
        switch (slot)
        {
            case 0: AbilityCelestialStrike(); break;
            case 1: AbilityAbsoluteShield(); break;
            case 2: AbilityBattleCry(); break;
        }
    }

    void AbilityCelestialStrike()
    {
        if (playerController != null) 
        {
            StartCoroutine(CelestialStrikeCoroutine());
        }
    }

    IEnumerator CelestialStrikeCoroutine()
    {
        LayerMask enemyLayer = LayerMask.GetMask("Enemy");
        Vector3 playerPos = playerController.transform.position;

        for (int i = 0; i < celestialStrikeCount; i++)
        {
            float randomX = Random.Range(-celestialStrikeRange, celestialStrikeRange);
            
            // Oyuncunun ayak hizasına senin verdiğin el yapımı ofseti ekliyoruz
            Vector3 strikePosition = new Vector3(playerPos.x + randomX, playerPos.y + celestialStrikeYOffset, 0f);

            if (celestialStrikeVFX != null)
            {
                GameObject vfx = Instantiate(celestialStrikeVFX, strikePosition, Quaternion.identity);
                
                // Animator engelini aşmak için hem parent'ı hem child'ları kökten büyütüyoruz
                SetVFXScaleRecursively(vfx, celestialStrikeScale);
                
                SetVFXSortingBehindPlayer(vfx); 
                Destroy(vfx, 2f); 
            }

            if (audioSource != null && celestialStrikeSFX != null)
            {
                audioSource.pitch = Random.Range(0.85f, 1.15f); 
                audioSource.PlayOneShot(celestialStrikeSFX, 0.6f); 
            }

            Collider2D[] enemies = Physics2D.OverlapCircleAll(strikePosition, strikeDamageRadius, enemyLayer);
            foreach (Collider2D enemy in enemies)
            {
                EnemyStats stats = enemy.GetComponent<EnemyStats>();
                if (stats != null) stats.TakeDamage(100);
            }

            yield return new WaitForSeconds(timeBetweenStrikes);
        }
    }

    void AbilityAbsoluteShield()
    {
        if (playerStats != null) StartCoroutine(ShieldCoroutine());
    }

    IEnumerator ShieldCoroutine()
    {
        playerStats.isInvincible = true;

        GameObject activeShield = null;
        if (absoluteShieldVFX != null && playerController != null)
        {
            activeShield = Instantiate(absoluteShieldVFX, playerController.transform.position, Quaternion.identity, playerController.transform);
            SetVFXSortingBehindPlayer(activeShield); 
        }

        if (audioSource != null && absoluteShieldSFX != null)
        {
            audioSource.pitch = 1f; 
            audioSource.PlayOneShot(absoluteShieldSFX, 0.7f);
        }

        yield return new WaitForSeconds(4f);

        if (playerStats != null) playerStats.isInvincible = false;
        if (activeShield != null) Destroy(activeShield);
    }

    void AbilityBattleCry()
    {
        if (playerController != null) StartCoroutine(BattleCryCoroutine());
    }

    IEnumerator BattleCryCoroutine()
    {
        if (battleCryVFX != null && playerController != null)
        {
            GameObject vfx = Instantiate(battleCryVFX, playerController.transform.position, Quaternion.identity, playerController.transform);
            SetVFXSortingBehindPlayer(vfx);
            Destroy(vfx, 3f); 
        }

        if (audioSource != null && battleCrySFX != null)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(battleCrySFX, 0.8f); 
        }

        float originalCooldown = playerController.attackCooldown;
        playerController.attackCooldown = originalCooldown / 2f; 
        playerController.attackDamage += 20; 
        
        yield return new WaitForSeconds(5f);
        
        if (playerController != null) {
            playerController.attackCooldown = originalCooldown;
            playerController.attackDamage -= 20;
        }
    }

    private void SetVFXSortingBehindPlayer(GameObject vfxObject)
    {
        if (playerController == null) return;

        SpriteRenderer playerSR = playerController.GetComponent<SpriteRenderer>();
        if (playerSR == null) return;

        SpriteRenderer[] vfxRenderers = vfxObject.GetComponentsInChildren<SpriteRenderer>();
        
        foreach (SpriteRenderer sr in vfxRenderers)
        {
            sr.sortingLayerID = playerSR.sortingLayerID; 
            sr.sortingOrder = playerSR.sortingOrder - 1; 
        }
    }

    // --- YENİ: ANIMATOR ENGELİNİ AŞAN AGRESİF BÜYÜTME FONKSİYONU ---
    private void SetVFXScaleRecursively(GameObject obj, float targetScale)
    {
        obj.transform.localScale = new Vector3(targetScale, targetScale, 1f);
        
        // Eğer alt objeler varsa hepsini tek tek gez ve boyutunu sabitle
        foreach (Transform child in obj.transform)
        {
            child.localScale = new Vector3(1f, 1f, 1f); // Parent büyüdüğü için child'ları bozmasın diye 1'e sabitliyoruz
            SetVFXScaleRecursively(child.gameObject, targetScale);
        }
    }
}

public class ActiveAbility
{
    public string name;
    public float cooldown;
}