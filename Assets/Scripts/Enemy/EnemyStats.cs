using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Düşman İstatistikleri")]
    public int maxHP = 30;
    public int currentHP;
    public int damage = 5;
    public int xpReward = 20;
    public float moveSpeed = 2f;
    public int goldReward = 10;

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

    // Hasar geri bildirimi (kırmızı flash)
    StartCoroutine(DamageFlash());

    if (currentHP <= 0)
    {
        Die();
    }
}

System.Collections.IEnumerator DamageFlash()
{
    SpriteRenderer sr = GetComponent<SpriteRenderer>();
    if (sr == null) yield break;

    Color originalColor = sr.color;
    sr.color = Color.red;
    yield return new WaitForSeconds(0.1f);
    sr.color = originalColor;
}

    void Die()
{
    isDead = true;

    PlayerStats player = FindFirstObjectByType<PlayerStats>();
    if (player != null)
    {
        player.GainXP(xpReward);
        player.GainGold(goldReward);
        player.AddKill();
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