using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Düşman İstatistikleri")]
    public int maxHP = 30;
    public int currentHP;
    public int damage = 5;
    public int xpReward = 20;
    public float moveSpeed = 2f;

    private bool isDead = false;

    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        currentHP -= damageAmount;
        Debug.Log($"Düşman {damageAmount} hasar aldı! HP: {currentHP}/{maxHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        PlayerStats player = FindFirstObjectByType<PlayerStats>();
        if (player != null)
        {
            player.GainXP(xpReward);
        }

        WaveManager waveManager = FindFirstObjectByType<WaveManager>();
        if (waveManager != null)
        {
            waveManager.OnEnemyDied();
        }

        Destroy(gameObject, 0.1f);
    }

    public float GetHPPercent()
    {
        return (float)currentHP / maxHP;
    }
}