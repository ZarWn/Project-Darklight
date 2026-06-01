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

    [Header("Göksel Çarpma Ayarları")]
    public int celestialStrikeCount = 4;       
    public float celestialStrikeRange = 8f;     
    public float timeBetweenStrikes = 0.25f;    
    public float strikeDamageRadius = 2.5f;     
    public float celestialStrikeYOffset = 1f;   
    public float celestialStrikeScale = 5f;     

    public delegate void OnAbilityCast(int slot);
    public static event OnAbilityCast onAbilityCast;

    public delegate void OnCooldownChanged(int slot, float remaining, float total);
    public static event OnCooldownChanged onCooldownChanged;

    private GameObject currentActiveShield;
    private bool isBattleCryActive = false;
    private float originalAttackCooldown;
    
    private int battleCryDamageBonus = 10;

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

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopAllCoroutines();
        if (audioSource != null) audioSource.Stop();
        
        if (currentActiveShield != null) Destroy(currentActiveShield);
        
        if (PlayerStats.Instance != null) 
        {
            PlayerStats.Instance.isInvincible = false;
            PlayerStats.Instance.isShielded = false; 
        }

        if (isBattleCryActive && PlayerController.Instance != null)
        {
            PlayerController.Instance.attackCooldown = originalAttackCooldown;
            PlayerController.Instance.attackDamage -= battleCryDamageBonus;
            isBattleCryActive = false;
        }
        
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
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void Update()
    {
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
        if (Time.timeScale == 0f) return;
        if (PlayerStats.Instance == null || PlayerStats.Instance.currentHP <= 0) return;

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
        if (PlayerController.Instance != null) 
            StartCoroutine(CelestialStrikeCoroutine());
    }

    IEnumerator CelestialStrikeCoroutine()
    {
        LayerMask enemyLayer = LayerMask.GetMask("Enemy");
        Vector3 playerPos = PlayerController.Instance.transform.position;

        int strikeDamage = PlayerController.Instance != null ? Mathf.RoundToInt(PlayerController.Instance.attackDamage * 2.5f) : 30;

        for (int i = 0; i < celestialStrikeCount; i++)
        {
            if (PlayerStats.Instance == null || PlayerStats.Instance.currentHP <= 0) break;

            float randomX = Random.Range(-celestialStrikeRange, celestialStrikeRange);
            Vector3 strikePosition = new Vector3(playerPos.x + randomX, playerPos.y + celestialStrikeYOffset, 0f);

            if (celestialStrikeVFX != null)
            {
                GameObject vfx = Instantiate(celestialStrikeVFX, strikePosition, Quaternion.identity);
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
                if (enemy.TryGetComponent(out EnemyStats stats)) stats.TakeDamage(strikeDamage);
            }

            yield return new WaitForSecondsRealtime(timeBetweenStrikes);
        }
    }

    void AbilityAbsoluteShield()
    {
        if (PlayerStats.Instance != null) StartCoroutine(ShieldCoroutine());
    }

    IEnumerator ShieldCoroutine()
    {
        PlayerStats.Instance.isShielded = true;

        if (currentActiveShield != null) Destroy(currentActiveShield);

        if (absoluteShieldVFX != null && PlayerController.Instance != null)
        {
            currentActiveShield = Instantiate(absoluteShieldVFX, PlayerController.Instance.transform.position, Quaternion.identity, PlayerController.Instance.transform);
            SetVFXSortingBehindPlayer(currentActiveShield); 
        }

        if (audioSource != null && absoluteShieldSFX != null)
        {
            audioSource.pitch = 1f; 
            audioSource.PlayOneShot(absoluteShieldSFX, 0.7f);
        }

        yield return new WaitForSecondsRealtime(4f);

        if (PlayerStats.Instance != null) 
        {
            PlayerStats.Instance.isShielded = false; 
        }
        if (currentActiveShield != null) Destroy(currentActiveShield);
    }

    void AbilityBattleCry()
    {
        if (PlayerController.Instance != null) StartCoroutine(BattleCryCoroutine());
    }

    IEnumerator BattleCryCoroutine()
    {
        if (battleCryVFX != null && PlayerController.Instance != null)
        {
            GameObject vfx = Instantiate(battleCryVFX, PlayerController.Instance.transform.position, Quaternion.identity, PlayerController.Instance.transform);
            SetVFXSortingBehindPlayer(vfx);
            Destroy(vfx, 3f); 
        }

        if (audioSource != null && battleCrySFX != null)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(battleCrySFX, 0.8f); 
        }

        if (PlayerController.Instance == null || isBattleCryActive) yield break;

        originalAttackCooldown = PlayerController.Instance.attackCooldown;
        
        PlayerController.Instance.attackCooldown = originalAttackCooldown * 0.7f; 
        PlayerController.Instance.attackDamage += battleCryDamageBonus; 
        isBattleCryActive = true;
        
        yield return new WaitForSecondsRealtime(5f);
        
        if (PlayerController.Instance != null && isBattleCryActive) 
        {
            PlayerController.Instance.attackCooldown = originalAttackCooldown;
            PlayerController.Instance.attackDamage -= battleCryDamageBonus;
            isBattleCryActive = false;
        }
    }

    private void SetVFXSortingBehindPlayer(GameObject vfxObject)
    {
        if (PlayerController.Instance == null) return;

        SpriteRenderer playerSR = PlayerController.Instance.GetComponent<SpriteRenderer>();
        if (playerSR == null) return;

        SpriteRenderer[] vfxRenderers = vfxObject.GetComponentsInChildren<SpriteRenderer>();
        
        foreach (SpriteRenderer sr in vfxRenderers)
        {
            sr.sortingLayerID = playerSR.sortingLayerID; 
            sr.sortingOrder = playerSR.sortingOrder - 1; 
        }
    }

    private void SetVFXScaleRecursively(GameObject obj, float targetScale)
    {
        obj.transform.localScale = new Vector3(targetScale, targetScale, 1f);
        foreach (Transform child in obj.transform)
        {
            child.localScale = new Vector3(1f, 1f, 1f); 
            SetVFXScaleRecursively(child.gameObject, targetScale);
        }
    }
}

// İŞTE UNUTTUKLARIMIZ! BU KISIM DOSYANIN EN ALTINDA OLMALI:
public class ActiveAbility
{
    public string name;
    public float cooldown;
}