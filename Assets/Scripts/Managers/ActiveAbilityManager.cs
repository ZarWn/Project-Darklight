using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ActiveAbilityManager : MonoBehaviour
{
    public static ActiveAbilityManager Instance;

    [Header("Slotlar")]
    public Button slot1Button;
    public Button slot2Button;
    public Button slot3Button;
    public Button slot4Button;

    [Header("Slot Textleri")]
    public TextMeshProUGUI slot1Text;
    public TextMeshProUGUI slot2Text;
    public TextMeshProUGUI slot3Text;
    public TextMeshProUGUI slot4Text;

    [Header("Cooldown Textleri")]
    public TextMeshProUGUI slot1Cooldown;
    public TextMeshProUGUI slot2Cooldown;
    public TextMeshProUGUI slot3Cooldown;
    public TextMeshProUGUI slot4Cooldown;

    private PlayerStats playerStats;
    private PlayerController playerController;

    // Yetenek verileri
    public class ActiveAbility
    {
        public string name;
        public float cooldown;
        public float currentCooldown;
        public bool isOnCooldown;
        public int abilityIndex;

        public ActiveAbility(string name, float cooldown, int index)
        {
            this.name = name;
            this.cooldown = cooldown;
            this.currentCooldown = 0f;
            this.isOnCooldown = false;
            this.abilityIndex = index;
        }
    }

    // Tüm aktif yetenekler
    public static string[] AbilityNames = {
        "Ates Topu",
        "Kalkan",
        "Hiz Artisi",
        "Simşek",
        "Can Calma",
        "Buz Tuzagi",
        "Güc Dalgasi",
        "Karanlık Perde"
    };

    public static float[] AbilityCooldowns = {
        8f,   // Ateş Topu
        15f,  // Kalkan
        12f,  // Hız Artışı
        20f,  // Şimşek
        10f,  // Can Çalma
        14f,  // Buz Tuzağı
        18f,  // Güç Dalgası
        25f   // Karanlık Perde
    };

    public static string[] AbilityDescs = {
        "Tüm düşmanlara 50 hasar",
        "5sn hasar almaz",
        "5sn saldiri hizi 2x",
        "Tüm düşmanlara 30 hasar",
        "Yakin düşmanlardan can cal",
        "Düşmanlari yavaşlat",
        "Hasar gecici olarak 2x artar",
        "5sn görünmez ol"
    };

    // Slotlardaki yetenekler (-1 = boş)
    private ActiveAbility[] slots = new ActiveAbility[4];
    private float[] currentCooldowns = new float[4];
    private bool[] isOnCooldown = new bool[4];

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
        playerController = FindFirstObjectByType<PlayerController>();
        UpdateAllSlotUI();
    }

    void Update()
    {
        playerStats = PlayerStats.Instance;
        playerController = PlayerController.Instance;
        UpdateCooldowns();
    }

    // Slota yetenek ekle (shoptan çağrılır)
    public bool AddAbilityToSlot(int abilityIndex)
    {
        // Boş slot bul
        for (int i = 0; i < 4; i++)
        {
            if (slots[i] == null)
            {
                slots[i] = new ActiveAbility(
                    AbilityNames[abilityIndex],
                    AbilityCooldowns[abilityIndex],
                    abilityIndex
                );
                UpdateSlotUI(i);
                Debug.Log($"Yetenek eklendi: {AbilityNames[abilityIndex]} -> Slot {i + 1}");
                return true;
            }
        }
        Debug.Log("Tüm slotlar dolu!");
        return false;
    }

    // Slotu temizle
    public void ClearSlot(int slotIndex)
    {
        slots[slotIndex] = null;
        currentCooldowns[slotIndex] = 0f;
        isOnCooldown[slotIndex] = false;
        UpdateSlotUI(slotIndex);
    }

    void UpdateCooldowns()
    {
        for (int i = 0; i < 4; i++)
        {
            if (isOnCooldown[i])
            {
                currentCooldowns[i] -= Time.deltaTime;

                if (currentCooldowns[i] <= 0)
                {
                    currentCooldowns[i] = 0;
                    isOnCooldown[i] = false;
                    SetSlotInteractable(i, true);
                    UpdateCooldownText(i, "");
                }
                else
                {
                    UpdateCooldownText(i, $"{Mathf.Ceil(currentCooldowns[i])}s");
                }
            }
        }
    }

    void StartCooldown(int slotIndex)
    {
        if (slots[slotIndex] == null) return;
        isOnCooldown[slotIndex] = true;
        currentCooldowns[slotIndex] = slots[slotIndex].cooldown;
        SetSlotInteractable(slotIndex, false);
    }

    void SetSlotInteractable(int index, bool interactable)
    {
        Button btn = GetSlotButton(index);
        if (btn != null) btn.interactable = interactable;
    }

    Button GetSlotButton(int index)
    {
        switch (index)
        {
            case 0: return slot1Button;
            case 1: return slot2Button;
            case 2: return slot3Button;
            case 3: return slot4Button;
            default: return null;
        }
    }

    void UpdateCooldownText(int index, string text)
    {
        switch (index)
        {
            case 0: if (slot1Cooldown != null) slot1Cooldown.text = text; break;
            case 1: if (slot2Cooldown != null) slot2Cooldown.text = text; break;
            case 2: if (slot3Cooldown != null) slot3Cooldown.text = text; break;
            case 3: if (slot4Cooldown != null) slot4Cooldown.text = text; break;
        }
    }

    void UpdateSlotUI(int index)
    {
        TextMeshProUGUI slotText = null;
        switch (index)
        {
            case 0: slotText = slot1Text; break;
            case 1: slotText = slot2Text; break;
            case 2: slotText = slot3Text; break;
            case 3: slotText = slot4Text; break;
        }

        if (slotText != null)
        {
            if (slots[index] != null)
                slotText.text = slots[index].name;
            else
                slotText.text = "BOS";
        }
    }

    void UpdateAllSlotUI()
    {
        for (int i = 0; i < 4; i++)
            UpdateSlotUI(i);
    }

    // Butonlardan çağrılır
    public void UseSlot1() { UseAbility(0); }
    public void UseSlot2() { UseAbility(1); }
    public void UseSlot3() { UseAbility(2); }
    public void UseSlot4() { UseAbility(3); }

    void UseAbility(int slotIndex)
    {
        if (slots[slotIndex] == null) return;
        if (isOnCooldown[slotIndex]) return;

        int abilityIndex = slots[slotIndex].abilityIndex;
        ApplyAbility(abilityIndex);
        StartCooldown(slotIndex);

        Debug.Log($"Yetenek kullanildi: {slots[slotIndex].name}");

        // EVO kontrolü
        CheckEVO();
    }

    void ApplyAbility(int abilityIndex)
    {
        LayerMask enemyLayer = LayerMask.GetMask("Enemy");

        switch (abilityIndex)
        {
            case 0: // Ateş Topu - tüm düşmanlara 50 hasar
                Collider2D[] enemies = Physics2D.OverlapCircleAll(
                    playerController.transform.position, 10f, enemyLayer
                );
                foreach (Collider2D e in enemies)
                {
                    EnemyStats es = e.GetComponent<EnemyStats>();
                    if (es != null) es.TakeDamage(50);
                }
                break;

            case 1: // Kalkan - 5sn hasar almaz
                StartCoroutine(ActivateShield(5f));
                break;

            case 2: // Hız Artışı - 5sn saldırı hızı 2x
                StartCoroutine(ActivateSpeedBoost(5f));
                break;

            case 3: // Şimşek - tüm düşmanlara 30 hasar
                Collider2D[] allEnemies = Physics2D.OverlapCircleAll(
                    playerController.transform.position, 10f, enemyLayer
                );
                foreach (Collider2D e in allEnemies)
                {
                    EnemyStats es = e.GetComponent<EnemyStats>();
                    if (es != null) es.TakeDamage(30);
                }
                break;

            case 4: // Can Çalma
                StartCoroutine(ActivateLifeSteal(5f));
                break;

            case 5: // Buz Tuzağı - düşmanları yavaşlat
                StartCoroutine(SlowEnemies(5f));
                break;

            case 6: // Güç Dalgası - hasar 2x
                StartCoroutine(ActivatePowerSurge(5f));
                break;

            case 7: // Karanlık Perde - görünmez ol
                StartCoroutine(ActivateInvisibility(5f));
                break;
        }
    }

    IEnumerator ActivateShield(float duration)
    {
        if (playerStats != null) playerStats.isInvincible = true;
        Debug.Log("Kalkan aktif!");
        yield return new WaitForSeconds(duration);
        if (playerStats != null) playerStats.isInvincible = false;
        Debug.Log("Kalkan sona erdi!");
    }

    IEnumerator ActivateSpeedBoost(float duration)
    {
        if (playerController == null) yield break;
        float original = playerController.attackCooldown;
        playerController.attackCooldown /= 2f;
        Debug.Log("Hiz artisi aktif!");
        yield return new WaitForSeconds(duration);
        playerController.attackCooldown = original;
        Debug.Log("Hiz artisi sona erdi!");
    }

    IEnumerator ActivateLifeSteal(float duration)
    {
        float elapsed = 0f;
        LayerMask enemyLayer = LayerMask.GetMask("Enemy");

        while (elapsed < duration)
        {
            Collider2D[] nearby = Physics2D.OverlapCircleAll(
                playerController.transform.position, 2f, enemyLayer
            );
            foreach (Collider2D e in nearby)
            {
                EnemyStats es = e.GetComponent<EnemyStats>();
                if (es != null)
                {
                    es.TakeDamage(5);
                    if (playerStats != null) playerStats.HealHP(5);
                }
            }
            yield return new WaitForSeconds(0.5f);
            elapsed += 0.5f;
        }
    }

    IEnumerator SlowEnemies(float duration)
    {
        LayerMask enemyLayer = LayerMask.GetMask("Enemy");
        Collider2D[] enemies = Physics2D.OverlapCircleAll(
            playerController.transform.position, 10f, enemyLayer
        );

        // Düşmanları yavaşlat
        List<EnemyStats> slowedEnemies = new List<EnemyStats>();
        foreach (Collider2D e in enemies)
        {
            EnemyStats es = e.GetComponent<EnemyStats>();
            if (es != null)
            {
                es.moveSpeed /= 2f;
                slowedEnemies.Add(es);
            }
        }

        Debug.Log("Düşmanlar yavaşlatildi!");
        yield return new WaitForSeconds(duration);

        // Hızlarını geri ver
        foreach (EnemyStats es in slowedEnemies)
        {
            if (es != null)
                es.moveSpeed *= 2f;
        }
        Debug.Log("Yavaşlatma sona erdi!");
    }

    IEnumerator ActivatePowerSurge(float duration)
    {
        if (playerController == null) yield break;
        int bonusDamage = playerController.attackDamage;
        playerController.attackDamage *= 2;
        Debug.Log("Güc dalgasi aktif!");
        yield return new WaitForSeconds(duration);
        playerController.attackDamage = bonusDamage;
        Debug.Log("Güc dalgasi sona erdi!");
    }

    IEnumerator ActivateInvisibility(float duration)
    {
        if (playerStats != null) playerStats.isInvincible = true;
        SpriteRenderer sr = playerController.GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = new Color(1f, 1f, 1f, 0.3f);
        Debug.Log("Görünmezlik aktif!");
        yield return new WaitForSeconds(duration);
        if (playerStats != null) playerStats.isInvincible = false;
        if (sr != null) sr.color = Color.white;
        Debug.Log("Görünmezlik sona erdi!");
    }

    // EVO Sistemi
    void CheckEVO()
    {
        // Ateş Topu (0) + Şimşek (3) = Kıyamet
        if (HasAbility(0) && HasAbility(3))
        {
            Debug.Log("EVO: Kiyamet!");
            ActivateEVO_Kiyamet();
        }

        // Kalkan (1) + Hız Artışı (2) = Savaş Modu
        if (HasAbility(1) && HasAbility(2))
        {
            Debug.Log("EVO: Savas Modu!");
            ActivateEVO_SavasModu();
        }

        // Can Çalma (4) + Kalkan (1) = Ölümsüzlük
        if (HasAbility(4) && HasAbility(1))
        {
            Debug.Log("EVO: Ölümsüzlük!");
            ActivateEVO_Olümsüzlük();
        }
    }

    bool HasAbility(int abilityIndex)
    {
        foreach (ActiveAbility slot in slots)
        {
            if (slot != null && slot.abilityIndex == abilityIndex)
                return true;
        }
        return false;
    }

    void ActivateEVO_Kiyamet()
    {
        StartCoroutine(EVO_Kiyamet());
    }

    IEnumerator EVO_Kiyamet()
    {
        LayerMask enemyLayer = LayerMask.GetMask("Enemy");
        float duration = 5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Collider2D[] enemies = Physics2D.OverlapCircleAll(
                playerController.transform.position, 10f, enemyLayer
            );
            foreach (Collider2D e in enemies)
            {
                EnemyStats es = e.GetComponent<EnemyStats>();
                if (es != null) es.TakeDamage(20);
            }
            yield return new WaitForSeconds(0.5f);
            elapsed += 0.5f;
        }
    }

    void ActivateEVO_SavasModu()
    {
        StartCoroutine(EVO_SavasModu());
    }

    IEnumerator EVO_SavasModu()
    {
        if (playerStats != null) playerStats.isInvincible = true;
        if (playerController != null)
            playerController.attackCooldown /= 2f;

        yield return new WaitForSeconds(8f);

        if (playerStats != null) playerStats.isInvincible = false;
        if (playerController != null)
            playerController.attackCooldown *= 2f;
    }

    void ActivateEVO_Olümsüzlük()
    {
        StartCoroutine(EVO_Olümsüzlük());
    }

    IEnumerator EVO_Olümsüzlük()
    {
        if (playerStats != null) playerStats.isInvincible = true;
        float elapsed = 0f;
        LayerMask enemyLayer = LayerMask.GetMask("Enemy");

        while (elapsed < 8f)
        {
            Collider2D[] nearby = Physics2D.OverlapCircleAll(
                playerController.transform.position, 3f, enemyLayer
            );
            foreach (Collider2D e in nearby)
            {
                EnemyStats es = e.GetComponent<EnemyStats>();
                if (es != null)
                {
                    es.TakeDamage(10);
                    if (playerStats != null) playerStats.HealHP(5);
                }
            }
            yield return new WaitForSeconds(0.5f);
            elapsed += 0.5f;
        }

        if (playerStats != null) playerStats.isInvincible = false;
    }
}