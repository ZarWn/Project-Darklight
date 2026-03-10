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

    [Header("Events")]
    public UnityEvent onLevelUp;
    public UnityEvent onPlayerDeath;

    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        Debug.Log($"Oyuncu {damage} hasar aldı! HP: {currentHP}/{maxHP}");

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
    }

    void Die()
    {
        Debug.Log("Oyuncu öldü! GAME OVER");
        onPlayerDeath?.Invoke();
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