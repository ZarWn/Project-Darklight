using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [Header("Saldırı Ayarları")]
    public float attackRange = 3f, attackCooldown = 0.5f, hitDelay = 0.2f;
    public int attackDamage = 10;
    public LayerMask enemyLayer;

    [Header("Sesler")]
    public AudioClip swordAttackSound;
    private AudioSource audioSource;
    private Animator animator;
    private WeaponData currentWeapon;

    private float attackTimer;

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = gameObject.GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();

        enemyLayer = LayerMask.GetMask("Enemy");
        if (TryGetComponent(out Rigidbody2D rb))
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.mass = 1000f;
        }

        ApplyWeapon();
        attackTimer = attackCooldown;
    }

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f;
        ApplyWeapon();
        
        if (animator) 
        {
            animator.Rebind();
            animator.Update(0f);
        }
    }

    void ApplyWeapon()
    {
        // Eğer direkt GameScene'den başlarsan çökmeyi önleyen varsayılan değerler
        if (WeaponManager.Instance == null || WeaponManager.Instance.GetSelectedWeapon() == null) 
        {
            attackRange = 3f; 
            attackCooldown = 0.5f;
            return;
        }
        
        currentWeapon = WeaponManager.Instance.GetSelectedWeapon();
        attackDamage = currentWeapon.damage;
        
        // --- ASIL HATA ÇÖZÜMÜ: MENZİL KÖR NOKTASI ---
        // Düşman 1.5'te dururken silah menzili 1.5 olursa fizik motoru kıl payı kaçırıyordu.
        // Tolerans payı (+0.8f) ve minimum bir genişlik (2.5f) ekleyerek bu kör noktayı tamamen sildim.
        attackRange = Mathf.Max(2.5f, currentWeapon.range + 0.8f);
        
        attackCooldown = Mathf.Clamp(currentWeapon.attackSpeed, 0.2f, 3f);
        
        // Animasyon hızını silah hızına senkronize et ki çok hızlı silahlarda donmasın
        if (animator) animator.speed = Mathf.Clamp(0.5f / attackCooldown, 0.5f, 2.5f);
    }

    void Update()
    {
        attackTimer += Time.deltaTime;
        
        // Sadece cooldown bittiyse ve "GERÇEKTEN" düşman varsa vur
        if (attackTimer >= attackCooldown && CheckIfEnemyNearby())
        {
            attackTimer = 0f;
            FaceNearestEnemy();
            PerformAnimationAndSound();
            StartCoroutine(DelayedHitSequence());
        }
    }

    void PerformAnimationAndSound()
    {
        if (animator)
        {
            animator.ResetTrigger("Attack"); // Önceki saldırıdan kalan hayalet trigger'ı temizle
            animator.SetFloat("AttackIndex", Random.Range(0f, 1f));
            animator.SetTrigger("Attack");
        }
        if (audioSource && swordAttackSound)
        {
            audioSource.pitch = Random.Range(0.9f, 1.15f);
            audioSource.PlayOneShot(swordAttackSound, 0.6f);
        }
    }

    void FaceNearestEnemy()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
        if (enemies.Length == 0) return;

        Collider2D closest = enemies[0];
        float minDist = Vector2.Distance(transform.position, closest.transform.position);

        foreach (var e in enemies)
        {
            float d = Vector2.Distance(transform.position, e.transform.position);
            if (d < minDist) { minDist = d; closest = e; }
        }

        int dir = closest.transform.position.x >= transform.position.x ? 1 : -1;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * dir, transform.localScale.y, transform.localScale.z);
    }

    IEnumerator DelayedHitSequence()
    {
        yield return new WaitForSeconds(hitDelay);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out EnemyStats es)) es.TakeDamage(attackDamage);
        }
    }

    bool CheckIfEnemyNearby() => Physics2D.OverlapCircle(transform.position, attackRange, enemyLayer) != null;

    public void IncreaseAttackDamage(int amt) => attackDamage += amt;
    public void IncreaseAttackSpeed(float amt) => attackCooldown = Mathf.Max(0.1f, attackCooldown - amt);
    public void IncreaseAttackRange(float amt) => attackRange += amt;
    public void IncreaseFireDamage(int amt) => attackDamage += amt;
    public void ActivateSuperSpeed() => attackCooldown = Mathf.Max(0.1f, attackCooldown - 0.2f);
}