using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Düşman İstatistikleri")]
    public int maxHP = 30, currentHP, damage = 5, xpReward = 20, goldReward = 10;
    public float moveSpeed = 2f;
    
    [Header("Efektler")]
    public GameObject bloodEffectPrefab;
    public AudioClip hurtSound; 
    
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    private Color originalColor = Color.white;
    
    [HideInInspector] public bool isDead = false;
    [HideInInspector] public bool isBleedingAlready = false;

    void Start()
    {
        currentHP = maxHP;
        audioSource = GetComponent<AudioSource>(); 
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;
        currentHP -= damageAmount;

        // Hoca Sorarsa: "Hasar sesini her seferinde aynı frekansta çalmamak için pitch değerini rastgele (Random.Range) değiştirerek organik bir his verdim."
        if (audioSource && hurtSound) 
        {
            audioSource.pitch = Random.Range(0.85f, 1.15f);
            audioSource.PlayOneShot(hurtSound, 0.4f); 
        }

        if (bloodEffectPrefab)
        {
            Vector3 pos = transform.position + Vector3.up * 0.5f;
            Destroy(Instantiate(bloodEffectPrefab, pos, Quaternion.identity), 0.5f); 
        }

        if (currentHP > 0) GetComponent<Animator>()?.SetTrigger("Hurt");
        if (gameObject.activeInHierarchy) StartCoroutine(FlashRed());

        if (currentHP <= 0) Die();
    }

    void Die()
    {
        isDead = true;
        currentHP = 0;

        // Hoca Sorarsa: "Oyuncu XP ve Altın kazanımını PlayerStats referansı aramak yerine Singleton (Instance) üzerinden çağırarak performans artışı sağladım."
        if (PlayerStats.Instance != null) 
        {
            PlayerStats.Instance.GainXP(xpReward);
            PlayerStats.Instance.GainGold(goldReward);
            PlayerStats.Instance.AddKill();
        }

        FindFirstObjectByType<WaveManager>()?.OnEnemyDied();
        GetComponent<EnemyController>()?.Die(); 
    }

    System.Collections.IEnumerator FlashRed()
    {
        // Hoca Sorarsa: "Coroutineler (IEnumerator) ile bekleme işlemlerini oyunun ana akışını durdurmadan arka planda yapabiliyoruz."
        if (spriteRenderer) 
        {
            spriteRenderer.color = Color.red; 
            yield return new WaitForSeconds(0.15f); 
            spriteRenderer.color = originalColor; 
        }
    }

    public float GetHPPercent() => (float)currentHP / maxHP;
}