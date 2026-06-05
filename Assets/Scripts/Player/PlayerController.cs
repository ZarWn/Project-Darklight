using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [Header("Saldırı Modu Ayarları")]
    // YENİ: 'O' tuşu ile kontrol edeceğimiz şalter
    public bool otomatikSaldiriAcik = false; 

    [Header("Saldırı Ayarları")]
    public float attackRange = 3f, attackCooldown = 0.5f, hitDelay = 0.2f; //[cite: 4]
    public int attackDamage = 10; //[cite: 4]
    public LayerMask enemyLayer; //[cite: 4]

    [Header("Pasif Yetenekler")]
    public float lifestealChance = 0f; // Vampir Dişi kartı için[cite: 4]

    [Header("Sesler")]
    public AudioClip swordAttackSound; //[cite: 4]
    private AudioSource audioSource; //[cite: 4]
    private Animator animator; //[cite: 4]
    private WeaponData currentWeapon; //[cite: 4]

    private float attackTimer; //[cite: 4]

    private void Awake() //[cite: 4]
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); } //[cite: 4]
        else Destroy(gameObject); //[cite: 4]
    }

    void Start() //[cite: 4]
    {
        animator = GetComponent<Animator>(); //[cite: 4]
        audioSource = gameObject.GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>(); //[cite: 4]
        enemyLayer = LayerMask.GetMask("Enemy"); //[cite: 4]
        
        if (TryGetComponent(out Rigidbody2D rb)) //[cite: 4]
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; //[cite: 4]
            rb.mass = 1000f; //[cite: 4]
        }

        ApplyWeapon(); //[cite: 4]
        attackTimer = attackCooldown; //[cite: 4]
    }

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded; //[cite: 4]
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded; //[cite: 4]

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) //[cite: 4]
    {
        Time.timeScale = 1f; //[cite: 4]
        ApplyWeapon(); //[cite: 4]
        if (animator) { animator.Rebind(); animator.Update(0f); } //[cite: 4]
    }

    void ApplyWeapon() //[cite: 4]
    {
        if (WeaponManager.Instance == null || WeaponManager.Instance.GetSelectedWeapon() == null)  //[cite: 4]
        {
            attackRange = 3f; attackCooldown = 0.5f; return; //[cite: 4]
        }
        
        currentWeapon = WeaponManager.Instance.GetSelectedWeapon(); //[cite: 4]
        attackDamage = currentWeapon.damage; //[cite: 4]
        
        // Düşman hitbox kör noktasını engelleyen menzil toleransı[cite: 4]
        attackRange = Mathf.Max(2.5f, currentWeapon.range + 0.8f); //[cite: 4]
        
        // Silah bekleme süresini 0.3 ile 2.0 saniye arasında sınırla (aşırı hızları önler)[cite: 4]
        attackCooldown = Mathf.Clamp(currentWeapon.attackSpeed, 0.3f, 2.0f); //[cite: 4]
        
        if (animator)  //[cite: 4]
        {
            float targetAnimSpeed = 0.6f / attackCooldown; //[cite: 4]
            animator.speed = Mathf.Clamp(targetAnimSpeed, 0.8f, 1.5f); //[cite: 4]
        }
    }

    void Update()
    {
        // 1. ŞALTER: 'O' Tuşu ile Otomatik/Manuel Mod Değişimi
        if (Input.GetKeyDown(KeyCode.O))
        {
            otomatikSaldiriAcik = !otomatikSaldiriAcik;
        }

        attackTimer += Time.deltaTime;

        // 2. SALDIRI KONTROLÜ
        if (attackTimer >= attackCooldown)
        {
            if (otomatikSaldiriAcik)
            {
                // --- OTOMATİK MOD ---
                // Eskiden olduğu gibi, etrafta düşman varsa kendi vurur
                if (CheckIfEnemyNearby())
                {
                    attackTimer = 0f;
                    FaceNearestEnemy();
                    PerformAnimationAndSound();
                    StartCoroutine(DelayedHitSequence());
                }
            }
            else
            {
                // --- MANUEL MOD ---
                bool saldirdi = false;

                // Sola Vuruş (A veya Sol Ok tuşuna basılırsa)
                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    YonuDegistir(false); // Sola Dön
                    saldirdi = true;
                }
                // Sağa Vuruş (D veya Sağ Ok tuşuna basılırsa)
                else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    YonuDegistir(true); // Sağa Dön
                    saldirdi = true;
                }

                if (saldirdi)
                {
                    attackTimer = 0f;
                    PerformAnimationAndSound();
                    StartCoroutine(DelayedHitSequence());
                }
            }
        }
    }

    // YENİ: Manuel modda vuruş yönüne göre karakterin dönmesini sağlayan fonksiyon
    void YonuDegistir(bool sagaMi)
    {
        int dir = sagaMi ? 1 : -1;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * dir, transform.localScale.y, transform.localScale.z);
    }

    void PerformAnimationAndSound() //[cite: 4]
    {
        if (animator) //[cite: 4]
        {
            animator.ResetTrigger("Attack"); //[cite: 4]
            animator.SetFloat("AttackIndex", Random.Range(0f, 1f)); //[cite: 4]
            animator.SetTrigger("Attack"); //[cite: 4]
        }
        if (audioSource && swordAttackSound) //[cite: 4]
        {
            audioSource.pitch = Random.Range(0.9f, 1.15f); //[cite: 4]
            audioSource.PlayOneShot(swordAttackSound, 0.6f); //[cite: 4]
        }
    }

    void FaceNearestEnemy() //[cite: 4]
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer); //[cite: 4]
        if (enemies.Length == 0) return; //[cite: 4]

        Collider2D closest = enemies[0]; //[cite: 4]
        float minDist = Vector2.Distance(transform.position, closest.transform.position); //[cite: 4]

        foreach (var e in enemies) //[cite: 4]
        {
            float d = Vector2.Distance(transform.position, e.transform.position); //[cite: 4]
            if (d < minDist) { minDist = d; closest = e; } //[cite: 4]
        }

        int dir = closest.transform.position.x >= transform.position.x ? 1 : -1; //[cite: 4]
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * dir, transform.localScale.y, transform.localScale.z); //[cite: 4]
    }

    IEnumerator DelayedHitSequence() //[cite: 4]
    {
        yield return new WaitForSeconds(hitDelay); //[cite: 4]

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer); //[cite: 4]
        foreach (var hit in hits) //[cite: 4]
        {
            if (hit.TryGetComponent(out EnemyStats es)) //[cite: 4]
            {
                es.TakeDamage(attackDamage); //[cite: 4]

                // --- ALTIN ORAN: Vampirizm (Can Çalma) ---[cite: 4]
                if (lifestealChance > 0f && Random.value <= lifestealChance) //[cite: 4]
                {
                    if (PlayerStats.Instance != null) //[cite: 4]
                    {
                        int stealAmount = Mathf.Max(1, Mathf.RoundToInt(PlayerStats.Instance.maxHP * 0.025f)); //[cite: 4]
                        PlayerStats.Instance.HealHP(stealAmount); //[cite: 4]
                    }
                }
            }
        }
    }

    bool CheckIfEnemyNearby() => Physics2D.OverlapCircle(transform.position, attackRange, enemyLayer) != null; //[cite: 4]

    public void IncreaseAttackDamage(int amt) => attackDamage += amt; //[cite: 4]
    public void IncreaseAttackSpeed(float amt) => attackCooldown = Mathf.Max(0.1f, attackCooldown - amt); //[cite: 4]
    public void IncreaseAttackRange(float amt) => attackRange += amt; //[cite: 4]
    public void IncreaseFireDamage(int amt) => attackDamage += amt; //[cite: 4]
    public void ActivateSuperSpeed() => attackCooldown = Mathf.Max(0.1f, attackCooldown - 0.2f); //[cite: 4]
}