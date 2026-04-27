using UnityEngine;
using UnityEngine.Events;

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

    void Start()
    {
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

        StartCoroutine(DamageFlash());

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
        Debug.Log($"XP Kazanildi: +{amount} | Toplam: {currentXP}/{xpToNextLevel}");

        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            LevelUp();
        }
    }

    public void GainGold(int amount)
    {
        gold += amount;
        Debug.Log($"Altin Kazanildi: +{amount} | Toplam: {gold}");
    }

    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            Debug.Log($"Altin Harcandi: -{amount} | Kalan: {gold}");
            return true;
        }
        Debug.Log("Yeterli altin yok!");
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
        Debug.Log($"SEVİYE ATLANDI! Yeni Seviye: {currentLevel}");
        onLevelUp?.Invoke();

        LevelUpManager levelUpManager = FindFirstObjectByType<LevelUpManager>();
        if (levelUpManager != null)
            levelUpManager.ShowLevelUpPanel();
    }

    void Die()
    {
        Debug.Log("Oyuncu öldü! GAME OVER");
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
        Debug.Log($"Can yenilendi! HP: {currentHP}/{maxHP}");
    }

    public void IncreaseMaxHP(int amount)
    {
        maxHP += amount;
        currentHP += amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        Debug.Log($"Max can artti! Yeni Max HP: {maxHP}");
    }

    public void IncreaseArmor(int amount)
    {
        armor += amount;
        Debug.Log($"Zirh artti! Yeni Zirh: {armor}");
    }

    public float GetHPPercent()
    {
        return (float)currentHP / maxHP;
    }

    public float GetXPPercent()
    {
        return (float)currentXP / xpToNextLevel;
    }
}