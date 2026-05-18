using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class WaveManager : MonoBehaviour
{
    [Header("Spawn Noktaları")]
    public Transform leftSpawnPoint;
    public Transform rightSpawnPoint;

    [Header("Prefablar")]
    public GameObject normalEnemyPrefab;
    public GameObject eliteEnemyPrefab;
    public GameObject bossPrefab;

    [Header("Dalga Ayarları")]
    public int totalWaves = 5;
    public float timeBetweenWaves = 3f;
    public float spawnInterval = 0.8f;

    [Header("Dalga Güçlendirme")]
    public int baseEnemyCount = 3;
    public float enemyHPMultiplier = 1.3f;
    public float enemySpeedMultiplier = 1.1f;

    private int currentWave = 0;
    private int enemiesAlive = 0;
    private bool waveInProgress = false;
    private bool isBossFloor = false;
    private bool isEliteFloor = false;

    void Start()
    {
        // FloorManager'dan bilgi al
        if (FloorManager.Instance != null)
        {
            totalWaves = FloorManager.Instance.GetWavesForCurrentFloor();
            isBossFloor = FloorManager.Instance.IsCurrentFloorBoss();
            isEliteFloor = FloorManager.Instance.IsCurrentFloorElite();
        }

        StartCoroutine(StartNextWave());
    }

    IEnumerator StartNextWave()
    {
        yield return new WaitForSeconds(2f);

        // Katın içindeki tüm dalgaları tek tek dön
        while (currentWave < totalWaves)
        {
            currentWave++;
            
            // EĞER SON DALGAYSA VE BU BİR BOSS/ELITE KATIYSA DÜELLO BAŞLASIN
            if (currentWave == totalWaves && isBossFloor)
            {
                Debug.Log($"=== DALGA {currentWave}: BOSS DÜELLOSU BAŞLIYOR ===");
                yield return StartCoroutine(SpawnBoss());
            }
            else if (currentWave == totalWaves && isEliteFloor)
            {
                Debug.Log($"=== DALGA {currentWave}: ELİTE DÜELLOSU BAŞLIYOR ===");
                SpawnEliteEnemy();
            }
            // EĞER SON DALGA DEĞİLSE VEYA NORMAL BİR KATSA SÜRÜ GELSİN
            else
            {
                Debug.Log($"=== DALGA {currentWave} BAŞLIYOR ===");
                yield return StartCoroutine(SpawnWave(currentWave));
            }

            // Dalgadaki tüm düşmanların (veya Boss'un) ölmesini bekle
            yield return new WaitUntil(() => enemiesAlive <= 0);
            Debug.Log($"Dalga {currentWave} tamamlandi!");

            // Tüm dalgalar bitmediyse bir sonraki dalga için bekle
            if (currentWave < totalWaves)
                yield return new WaitForSeconds(timeBetweenWaves);
        }

        // Katın tüm dalgaları (ve varsa Boss'u) bittiyse katı tamamla
        FloorComplete();
    }

    IEnumerator SpawnWave(int waveNumber)
    {
        waveInProgress = true;

        // Kat başına düşman sayısı artar
        int floorBonus = FloorManager.Instance != null ?
            FloorManager.Instance.GetEnemyCountBonus() : 0;

        int enemyCount = baseEnemyCount + (waveNumber - 1) * 2 + floorBonus;
        enemiesAlive = enemyCount;

        for (int i = 0; i < enemyCount; i++)
        {
            Transform spawnPoint = (i % 2 == 0) ? rightSpawnPoint : leftSpawnPoint;
            SpawnEnemy(spawnPoint, waveNumber);
            yield return new WaitForSeconds(spawnInterval);
        }

        waveInProgress = false;
    }

    void SpawnEnemy(Transform spawnPoint, int waveNumber)
    {
        GameObject enemy = Instantiate(
            normalEnemyPrefab,
            spawnPoint.position,
            Quaternion.identity
        );

        EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
        if (enemyStats != null)
        {
            // Dalga güçlendirmesi
            float hpMultiplier = Mathf.Pow(enemyHPMultiplier, waveNumber - 1);

            // Kat güçlendirmesi
            float floorHPMult = FloorManager.Instance != null ?
                FloorManager.Instance.GetEnemyHPMultiplier() : 1f;
            float floorSpeedMult = FloorManager.Instance != null ?
                FloorManager.Instance.GetEnemySpeedMultiplier() : 1f;

            enemyStats.maxHP = Mathf.RoundToInt(
                enemyStats.maxHP * hpMultiplier * floorHPMult
            );
            enemyStats.currentHP = enemyStats.maxHP;

            float speedMultiplier = Mathf.Pow(enemySpeedMultiplier, waveNumber - 1);
            enemyStats.moveSpeed *= speedMultiplier * floorSpeedMult;

            // Altın ve XP kat başına artar
            int floorIndex = FloorManager.Instance != null ?
                FloorManager.Instance.currentFloor : 1;
            enemyStats.xpReward = Mathf.RoundToInt(
                enemyStats.xpReward * (1 + floorIndex * 0.1f)
            );
            enemyStats.goldReward = Mathf.RoundToInt(
                enemyStats.goldReward * (1 + floorIndex * 0.1f)
            );
        }
    }

    void SpawnEliteEnemy()
    {
        // Elite düşman prefabı varsa onu kullan yoksa normal düşmanı güçlendir
        GameObject prefabToUse = eliteEnemyPrefab != null ?
            eliteEnemyPrefab : normalEnemyPrefab;

        GameObject elite = Instantiate(
            prefabToUse,
            rightSpawnPoint.position,
            Quaternion.identity
        );

        EnemyStats enemyStats = elite.GetComponent<EnemyStats>();
        if (enemyStats != null)
        {
            float floorHPMult = FloorManager.Instance != null ?
                FloorManager.Instance.GetEnemyHPMultiplier() : 1f;

            // Elite düşman 3x güçlü
            enemyStats.maxHP = Mathf.RoundToInt(enemyStats.maxHP * 3f * floorHPMult);
            enemyStats.currentHP = enemyStats.maxHP;
            enemyStats.damage = Mathf.RoundToInt(enemyStats.damage * 2f);
            enemyStats.moveSpeed *= 1.3f;
            enemyStats.xpReward *= 3;
            enemyStats.goldReward *= 3;

            // Elite düşmanı büyüt
            elite.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
        }

        enemiesAlive = 1;
    }

    IEnumerator SpawnBoss()
    {
        Debug.Log("!!! BOSS DALGA BAŞLIYOR !!!");

        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null)
            uiManager.ShowBossWarning();

        yield return new WaitForSeconds(2f);

        GameObject boss = Instantiate(
            bossPrefab,
            rightSpawnPoint.position,
            Quaternion.identity
        );

        EnemyStats bossStats = boss.GetComponent<EnemyStats>();
        if (bossStats != null)
        {
            float floorHPMult = FloorManager.Instance != null ?
                FloorManager.Instance.GetEnemyHPMultiplier() : 1f;
            bossStats.maxHP = Mathf.RoundToInt(bossStats.maxHP * floorHPMult);
            bossStats.currentHP = bossStats.maxHP;
            
            // Eğer özel bir boss prefabın yoksa ve normal iskeleti kullanıyorsan 
            // onu da boss boyutuna getirelim (İsteğe bağlı silebilirsin)
            boss.transform.localScale = new Vector3(2.5f, 2.5f, 1f); 
        }

        enemiesAlive = 1;
    }

    public void OnEnemyDied()
    {
        enemiesAlive--;
        Debug.Log($"Düşman öldü! Kalan: {enemiesAlive}");

        // "Kat tamamlama" işini yukarıdaki WaitUntil hallettiği için buradaki eski if bloğunu kaldırdık
    }

    void FloorComplete()
    {
        Debug.Log("KAT TAMAMLANDI!");

        UIManager uiManager = FindFirstObjectByType<UIManager>();
        if (uiManager != null)
            uiManager.ShowStageClear();

        if (FloorManager.Instance != null)
            FloorManager.Instance.OnFloorCompleted();
    }

    public int GetCurrentWave() => currentWave;
    public int GetTotalWaves() => totalWaves;
    public bool IsWaveInProgress() => waveInProgress;
}