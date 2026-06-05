using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Düşman İstatistikleri")]
    public int maxHP = 30, currentHP, damage = 5, xpReward = 20, goldReward = 10; //[cite: 12]
    public float moveSpeed = 2f; //[cite: 12]
    
    [Header("Dinamik Ölçeklendirme (DDA)")]
    // YENİ: Bu tik açık olan düşmanlar (Elit/Boss) oyuncunun gücüne göre canını ve zırhını artırır.
    public bool isEliteOrBoss = false; 
    public int minHitsToKill = 15; // Bu elit/boss, oyuncunun normal vuruşlarından en az kaç tane dayanmalı?
    
    // YENİ ZIRH SİSTEMİ: Elitler aldıkları hasarı sabit bir miktar düşürür
    [HideInInspector] public int armorValue = 0; 

    [Header("Efektler")]
    public GameObject bloodEffectPrefab; //[cite: 12]
    public AudioClip hurtSound;  //[cite: 12]
    
    private AudioSource audioSource; //[cite: 12]
    private SpriteRenderer spriteRenderer; //[cite: 12]
    private Color originalColor = Color.white; //[cite: 12]
    
    [HideInInspector] public bool isDead = false; //[cite: 12]
    [HideInInspector] public bool isBleedingAlready = false; //[cite: 12]

    void Start()
    {
        audioSource = GetComponent<AudioSource>();  //[cite: 12]
        spriteRenderer = GetComponent<SpriteRenderer>(); //[cite: 12]
        if (spriteRenderer != null) originalColor = spriteRenderer.color; //[cite: 12]
        
        // YENİ: Düşman doğduğunda eğer elitse canını ve zırhını oyuncuya göre ayarlar
        ScaleHealthWithPlayer(); 
        
        // currentHP atamasını ScaleHealthWithPlayer sonrasına taşıdık ki yeni MaxHP'yi alabilsin
        currentHP = maxHP; 
    }

    // YENİ EKLENEN DİNAMİK ZORLUK FONKSİYONU
    public void ScaleHealthWithPlayer()
    {
        if (!isEliteOrBoss) return;

        // Oyuncunun gücünü çekiyoruz
        PlayerController player = PlayerController.Instance;
        if (player == null) return;

        int playerDamage = player.attackDamage;

        // 1. DİNAMİK CAN HAVUZU:
        // Oyuncunun hasarına göre bu düşmanın "tek atılmayacak" kadar cana sahip olmasını garanti ediyoruz.
        int requiredHP = playerDamage * minHitsToKill;

        if (maxHP < requiredHP)
        {
            maxHP = requiredHP;
        }

        // 2. DİNAMİK ZIRH (ARMOR):
        // Boss veya Elitler, oyuncunun vuruş gücünün %10'u kadar zırh kazanır (Örn: 50 vuruyorsan her vuruşunda 5 hasar engeller).
        // Eğer oyuncu zayıfsa (Örn: 5 hasar), minimum 1 zırh kazanır.
        armorValue = Mathf.Max(1, Mathf.RoundToInt(playerDamage * 0.10f));
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return; //[cite: 12]

        // ZIRH HESAPLAMASI: Gelen hasardan zırhı çıkar. Ancak hasarın eksiye düşüp can doldurmasını engelle (Min 1 hasar alsın).
        int finalDamage = damageAmount;
        if (isEliteOrBoss)
        {
            finalDamage = Mathf.Max(1, damageAmount - armorValue);
        }

        currentHP -= finalDamage; 

        // Hoca Sorarsa: "Hasar sesini her seferinde aynı frekansta çalmamak için pitch değerini rastgele (Random.Range) değiştirerek organik bir his verdim."[cite: 12]
        if (audioSource && hurtSound)  //[cite: 12]
        {
            audioSource.pitch = Random.Range(0.85f, 1.15f); //[cite: 12]
            audioSource.PlayOneShot(hurtSound, 0.4f);  //[cite: 12]
        }

        if (bloodEffectPrefab) //[cite: 12]
        {
            Vector3 pos = transform.position + Vector3.up * 0.5f; //[cite: 12]
            Destroy(Instantiate(bloodEffectPrefab, pos, Quaternion.identity), 0.5f);  //[cite: 12]
        }

        if (currentHP > 0) GetComponent<Animator>()?.SetTrigger("Hurt"); //[cite: 12]
        if (gameObject.activeInHierarchy) StartCoroutine(FlashRed()); //[cite: 12]

        if (currentHP <= 0) Die(); //[cite: 12]
    }

    void Die() //[cite: 12]
    {
        isDead = true; //[cite: 12]
        currentHP = 0; //[cite: 12]

        // Hoca Sorarsa: "Oyuncu XP ve Altın kazanımını PlayerStats referansı aramak yerine Singleton (Instance) üzerinden çağırarak performans artışı sağladım."[cite: 12]
        if (PlayerStats.Instance != null)  //[cite: 12]
        {
            PlayerStats.Instance.GainXP(xpReward); //[cite: 12]
            PlayerStats.Instance.GainGold(goldReward); //[cite: 12]
            PlayerStats.Instance.AddKill(); //[cite: 12]
        }

        FindFirstObjectByType<WaveManager>()?.OnEnemyDied(); //[cite: 12]
        GetComponent<EnemyController>()?.Die();  //[cite: 12]
    }

    System.Collections.IEnumerator FlashRed() //[cite: 12]
    {
        // Hoca Sorarsa: "Coroutineler (IEnumerator) ile bekleme işlemlerini oyunun ana akışını durdurmadan arka planda yapabiliyoruz."[cite: 12]
        if (spriteRenderer)  //[cite: 12]
        {
            spriteRenderer.color = Color.red;  //[cite: 12]
            yield return new WaitForSeconds(0.15f);  //[cite: 12]
            spriteRenderer.color = originalColor;  //[cite: 12]
        }
    }

    public float GetHPPercent() => (float)currentHP / maxHP; //[cite: 12]
}