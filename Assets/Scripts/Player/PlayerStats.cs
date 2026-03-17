using UnityEngine;
using UnityEngine.Events;

public class PlayerStats : MonoBehaviour
{
    [Header("Temel İstatistikler")]
    public int maxHP = 100;
    public int currentHP;
    public int currentXP = 0;
    public int currentLevel = 1;
    public int xpToNextLevel = 100;

    [Header("Zırh")]
    public int armor = 0;

    [Header("Events")]
    public UnityEvent onLevelUp;
    public UnityEvent onPlayerDeath;

    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
    // Zırh hasarı azaltır
    int finalDamage = Mathf.Max(1, damage - armor);
    currentHP -= finalDamage;
    currentHP = Mathf.Clamp(currentHP, 0, maxHP);
    Debug.Log($"Oyuncu {finalDamage} hasar aldı! HP: {currentHP}/{maxHP}");

    if (currentHP <= 0)
    {
        Die();
    }
    }

    public void GainXP(int amount)
    {
        currentXP += amount;
        Debug.Log($"XP Kazanıldı: +{amount} | Toplam: {currentXP}/{xpToNextLevel}");

        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            LevelUp();
        }
    }

    void LevelUp()
    {
    currentLevel++;
    xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.2f);
    Debug.Log($"SEVİYE ATLANDI! Yeni Seviye: {currentLevel}");
    onLevelUp?.Invoke();

    // LevelUpManager'ı çağır
    LevelUpManager levelUpManager = FindFirstObjectByType<LevelUpManager>();
    if (levelUpManager != null)
    {
        levelUpManager.ShowLevelUpPanel();
    }
    }

    void Die()
    {
    Debug.Log("Oyuncu öldü! GAME OVER");
    UIManager uiManager = FindFirstObjectByType<UIManager>();
    if (uiManager != null)
    {
        uiManager.ShowGameOver();
    }
    onPlayerDeath?.Invoke();
    }

    public void HealHP(int amount)
    {
    currentHP += amount;
    currentHP = Mathf.Clamp(currentHP, 0, maxHP);
    Debug.Log($"Can yenilendi! HP: {currentHP}/{maxHP}");
    }
    public float GetHPPercent()
    {
        return (float)currentHP / maxHP;
    }

    public float GetXPPercent()
    {
        return (float)currentXP / xpToNextLevel;
    }

    public void IncreaseMaxHP(int amount)
    {
    maxHP += amount;
    currentHP += amount;
    currentHP = Mathf.Clamp(currentHP, 0, maxHP);
    Debug.Log($"Max can arttı! Yeni Max HP: {maxHP}");
    }

    public void IncreaseArmor(int amount)
    {
    armor += amount;
    Debug.Log($"Zırh arttı! Yeni Zırh: {armor}");
    }

}