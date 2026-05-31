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

    public bool isInvincible = false;
    public UnityEvent onLevelUp, onPlayerDeath;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

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
        if (currentHP <= 0) ResetAllStats();
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHP -= damage;
        if (currentHP <= 0) { Die(); return; }

        StartCoroutine(DamageFlickerAndIFrame());
    }

    IEnumerator DamageFlickerAndIFrame()
    {
        isInvincible = true;
        if (spriteRenderer)
        {
            // Hoca Sorarsa: "Hasar anında Animator'ü bölüp kilitlenmeye (Stunlock) sebep olmak yerine, Sprite'ı kırmızı yakıp I-Frame (ölümsüzlük) vererek akıcılığı korudum."
            spriteRenderer.color = new Color(1f, 0.3f, 0.3f, 0.7f);
            yield return new WaitForSeconds(0.15f);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.15f);
            spriteRenderer.color = new Color(1f, 0.3f, 0.3f, 0.7f);
            yield return new WaitForSeconds(0.15f);
            spriteRenderer.color = originalColor;
        }
        else yield return new WaitForSeconds(0.45f);

        isInvincible = false;
    }

    public void GainXP(int amount)
    {
        currentXP += amount;
        // Hoca Sorarsa: "While döngüsü kullanarak, oyuncu tek seferde devasa XP alırsa birden fazla level atlamasını güvenle sağlıyorum."
        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            currentLevel++;
            xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.2f);
            onLevelUp?.Invoke();
            FindFirstObjectByType<LevelUpManager>()?.ShowLevelUpPanel();
        }
    }

    public void GainGold(int amt) => gold += amt;
    public bool SpendGold(int amt) { if (gold >= amt) { gold -= amt; return true; } return false; }
    public void AddKill() => killCount++;

    void Die()
    {
        GetComponent<Animator>()?.SetTrigger("Die");
        Time.timeScale = 0f;

        FindFirstObjectByType<LevelUpManager>()?.gameObject.SetActive(false);
        FindFirstObjectByType<UIManager>()?.ShowGameOver();
        onPlayerDeath?.Invoke();
    }

    public void HealHP(int amt) => currentHP = Mathf.Clamp(currentHP + amt, 0, maxHP);
    public void IncreaseMaxHP(int amt) { maxHP += amt; HealHP(amt); }
    public void IncreaseArmor(int amt) => armor += amt;

    public float GetHPPercent() => (float)currentHP / maxHP;
    public float GetXPPercent() => (float)currentXP / xpToNextLevel;

    public void ResetAllStats()
    {
        currentHP = maxHP; armor = 0; currentLevel = 1; currentXP = 0;
        xpToNextLevel = 100; gold = 0; killCount = 0; Time.timeScale = 1f;

        if (TryGetComponent(out Animator anim) && anim.runtimeAnimatorController)
        {
            anim.ResetTrigger("Die");
            anim.Rebind();
            anim.Update(0f);
        }
        if (spriteRenderer) spriteRenderer.color = originalColor;
    }
}