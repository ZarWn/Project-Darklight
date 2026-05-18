using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement; 

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    [Header("Temel İstatistikler")]
    public int maxHP = 100;
    public int currentHP;
    public int currentXP = 0;
    public int currentLevel = 1;
    public int xpToNextLevel = 100;
    public int armor = 0;

    [Header("Ekonomi")]
    public int gold = 0;
    public int killCount = 0;

    [Header("Durum")]
    public bool isInvincible = false;

    [Header("Efektler")]
    public GameObject bloodEffectPrefab;

    [Header("Ses Efektleri")]
    public AudioClip hurtSound; // Oyuncu hasar alınca çalacak ses
    private AudioSource audioSource;

    [Header("Events")]
    public UnityEvent onLevelUp;
    public UnityEvent onPlayerDeath;

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

        currentHP = maxHP;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (currentHP <= 0)
        {
            ResetAllStats();
            Debug.Log("Oyun sıfırlandı, yeni koşu başlıyor!");
        }
        else
        {
            Animator anim = GetComponent<Animator>();
            if (anim != null) anim.Play("Idle");
            Debug.Log("Yeni odaya geçildi. Mevcut can: " + currentHP);
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Hoparlörü tanımla

        if (currentHP <= 0)
            currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        int finalDamage = Mathf.Max(1, damage - armor);
        currentHP -= finalDamage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        Debug.Log($"Oyuncu {finalDamage} hasar aldi! HP: {currentHP}/{maxHP}");

        // --- YENİ EKLENEN: OYUNCU HASAR SESİ ---
        if (audioSource != null && hurtSound != null)
        {
            audioSource.PlayOneShot(hurtSound);
        }
        // ---------------------------------------

        if (bloodEffectPrefab != null)
        {
            Vector3 bloodPosition = transform.position + new Vector3(0f, -0.5f, 0f);
            GameObject blood = Instantiate(bloodEffectPrefab, bloodPosition, Quaternion.identity);
            Destroy(blood, 0.5f); 
        }

        StartCoroutine(DamageFlash());

        if (currentHP > 0)
        {
            Animator anim = GetComponent<Animator>();
            if (anim != null) anim.SetTrigger("Hurt");
        }

        if (currentHP <= 0)
            Die();
    }

    System.Collections.IEnumerator DamageFlash()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) yield break;

        Color originalColor = sr.color;
        sr.color = Color.white;
        yield return new WaitForSeconds(0.1f);

        if (sr != null)
            sr.color = originalColor;
    }

    public void GainXP(int amount)
    {
        currentXP += amount;
        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            LevelUp();
        }
    }

    public void GainGold(int amount)
    {
        gold += amount;
    }

    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            return true;
        }
        return false;
    }

    public void AddKill()
    {
        killCount++;
    }

    void LevelUp()
    {
        currentLevel++;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.2f);
        onLevelUp?.Invoke();

        LevelUpManager levelUpManager = FindFirstObjectByType<LevelUpManager>();
        if (levelUpManager != null)
            levelUpManager.ShowLevelUpPanel();
    }

    void Die()
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null) anim.SetTrigger("Die");
        
        Time.timeScale = 0f;

        LevelUpManager levelUpManager = FindFirstObjectByType<LevelUpManager>();
        if (levelUpManager != null)
            levelUpManager.gameObject.SetActive(false);

        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null)
            uiManager.ShowGameOver();

        onPlayerDeath?.Invoke();
    }

    public void HealHP(int amount)
    {
        currentHP += amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
    }

    public void IncreaseMaxHP(int amount)
    {
        maxHP += amount;
        currentHP += amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
    }

    public void IncreaseArmor(int amount)
    {
        armor += amount;
    }

    public float GetHPPercent()
    {
        return (float)currentHP / maxHP;
    }

    public float GetXPPercent()
    {
        return (float)currentXP / xpToNextLevel;
    }

    public void ResetAllStats()
    {
        currentHP = maxHP; 
        armor = 0;
        currentLevel = 1;
        currentXP = 0;
        xpToNextLevel = 100;
        gold = 0;
        killCount = 0;
        Time.timeScale = 1f;

        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.ResetTrigger("Die");
            anim.Play("Idle"); 
        }
    }
}