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

    [Header("Efektler")]
    public GameObject bloodEffectPrefab;

    [Header("Ses Efektleri")]
    public AudioClip hurtSound; 
    private AudioSource audioSource;

    private SpriteRenderer spriteRenderer;
    private Color originalColor = Color.white;
    public bool isDead = false;

    [HideInInspector]
    public bool isBleedingAlready = false;

    void Start()
    {
        currentHP = maxHP;
        audioSource = GetComponent<AudioSource>(); 

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        currentHP -= damageAmount;

        // --- YENİ: DENGELİ HASAR SESİ ---
        if (audioSource != null && hurtSound != null)
        {
            // Her darbede farklı acı çığlığı tonu yakalamak için pitch ayarı
            audioSource.pitch = Random.Range(0.85f, 1.15f);
            // Hasar sesini %40 ses seviyesine (0.4f) sabitledik
            audioSource.PlayOneShot(hurtSound, 0.4f); 
        }

        if (bloodEffectPrefab != null)
        {
            Vector3 bloodPosition = transform.position + new Vector3(0f, 0.5f, 0f);
            GameObject blood = Instantiate(bloodEffectPrefab, bloodPosition, Quaternion.identity);
            Destroy(blood, 0.5f); 
        }

        if (currentHP > 0)
        {
            Animator anim = GetComponent<Animator>();
            if (anim != null) anim.SetTrigger("Hurt");
        }

        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(FlashRed());
        }

        if (currentHP <= 0)
        {
            currentHP = 0;
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

            EnemyController controller = GetComponent<EnemyController>();
            if (controller != null)
            {
                controller.Die(); 
            }
        }
    }

    System.Collections.IEnumerator FlashRed()
    {
        if (spriteRenderer != null)
        {
            scriptRenderer: spriteRenderer.color = Color.red; 
            yield return new WaitForSeconds(0.15f); 
            spriteRenderer.color = originalColor; 
        }
    }

    public float GetHPPercent()
    {
        return (float)currentHP / maxHP;
    }
}