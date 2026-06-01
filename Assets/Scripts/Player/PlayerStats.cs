using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement; 
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    [Header("Temel İstatistikler")]
    public int maxHP = 100, currentHP, armor = 0;
    public int currentXP = 0, currentLevel = 1, xpToNextLevel = 100;
    public int gold = 0, killCount = 0;

    [Header("Pasif Çarpanlar (Kartlardan Gelir)")]
    public float xpMultiplier = 1f;
    public float goldMultiplier = 1f;
    public float dodgeChance = 0f;

    [Header("Durum")]
    public bool isInvincible = false;
    // HATA ÇÖZÜMÜ: Kalkan yeteneğine özel yeni bir koruma durumu ekledik!
    public bool isShielded = false; 

    [Header("Efektler")]
    private SpriteRenderer spriteRenderer; 
    private Color originalColor;           

    [Header("Events")]
    public UnityEvent onLevelUp, onPlayerDeath;

    public GameObject dodgeTextPrefab; // YENİ EKLENDİ: Kaçınma yazısı kalıbı
    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer) originalColor = spriteRenderer.color;
        if (currentHP <= 0) currentHP = maxHP;
    }

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isInvincible = false; 
        isShielded = false; // Sahne değişince kalkan durumunu da sıfırla
        if (spriteRenderer) spriteRenderer.color = originalColor;
        if (currentHP <= 0) ResetAllStats();
    }

    public void TakeDamage(int damage)
    {
        // HATA ÇÖZÜMÜ: Eğer normal ölümsüzsen VEYA kalkanın açıksa hasarı iptal et!
        if (isInvincible || isShielded) return;

        float realDodge = Mathf.Clamp(dodgeChance, 0f, 0.8f);
        if (realDodge > 0f && Random.value <= realDodge) return;

        int finalDamage = Mathf.Max(1, damage - armor);
        currentHP -= finalDamage;
        
        if (currentHP <= 0) { Die(); return; }

        StartCoroutine(DamageFlickerAndIFrame());
    }

    IEnumerator DamageFlickerAndIFrame()
    {
        isInvincible = true;
        
        if (spriteRenderer)
        {
            spriteRenderer.color = new Color(1f, 0.3f, 0.3f, 0.7f);
            yield return new WaitForSecondsRealtime(0.15f);
            spriteRenderer.color = originalColor;
            yield return new WaitForSecondsRealtime(0.15f);
            spriteRenderer.color = new Color(1f, 0.3f, 0.3f, 0.7f);
            yield return new WaitForSecondsRealtime(0.15f);
            spriteRenderer.color = originalColor;
        }
        else yield return new WaitForSecondsRealtime(0.45f);

        // Bu sadece yanıp sönme ölümsüzlüğünü kapatacak, kalkanı ellemeyecek.
        isInvincible = false;
    }

    public void GainXP(int amount)
    {
        int finalXP = Mathf.RoundToInt(amount * xpMultiplier);
        currentXP += finalXP;

        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            currentLevel++;
            xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.15f);
            
            HealHP(Mathf.RoundToInt(maxHP * 0.25f));
            
            onLevelUp?.Invoke();
            
            LevelUpManager levelUpManager = FindFirstObjectByType<LevelUpManager>();
            if (levelUpManager != null) levelUpManager.ShowLevelUpPanel();
        }
    }

    public void GainGold(int amt) => gold += Mathf.RoundToInt(amt * goldMultiplier); 
    public bool SpendGold(int amt) { if (gold >= amt) { gold -= amt; return true; } return false; }
    
    public void AddKill()
    {
        killCount++;
        HealHP(Mathf.Max(1, Mathf.RoundToInt(maxHP * 0.02f)));
    }

    void Die()
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null) anim.SetTrigger("Die");
        
        Time.timeScale = 0f;

        LevelUpManager levelUpManager = FindFirstObjectByType<LevelUpManager>();
        if (levelUpManager != null) levelUpManager.gameObject.SetActive(false);
        
        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null) uiManager.ShowGameOver();
        
        onPlayerDeath?.Invoke();
    }

    public void HealHP(int amt) => currentHP = Mathf.Clamp(currentHP + amt, 0, maxHP);
    
    public void IncreaseMaxHP(int amt) 
    { 
        maxHP += amt; 
        if (maxHP < 1) maxHP = 1;
        HealHP(amt > 0 ? amt : 0); 
    }
    
    public void IncreaseArmor(int amt) => armor += amt;

    public float GetHPPercent() => (float)currentHP / maxHP;
    public float GetXPPercent() => (float)currentXP / xpToNextLevel;

    public void ResetAllStats()
    {
        currentHP = maxHP; armor = 0; currentLevel = 1; currentXP = 0;
        xpToNextLevel = 100; gold = 0; killCount = 0; Time.timeScale = 1f;

        xpMultiplier = 1f; goldMultiplier = 1f; dodgeChance = 0f;
        if (PlayerController.Instance != null) PlayerController.Instance.lifestealChance = 0f;

        if (TryGetComponent(out Animator anim) && anim.runtimeAnimatorController)
        {
            anim.ResetTrigger("Die");
            anim.Rebind();
            anim.Update(0f);
        }
        if (spriteRenderer) spriteRenderer.color = originalColor;
        isInvincible = false;
        isShielded = false; // Kalkanı da sıfırla
    }
}