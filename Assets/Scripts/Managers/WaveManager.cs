using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    [Header("Spawn Noktaları")]
    public Transform leftSpawnPoint;
    public Transform rightSpawnPoint;

    [Header("Prefablar")]
    public GameObject normalEnemyPrefab;
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
    private bool bossSpawned = false;

   void Start()
{
   
    if (StageManager.Instance != null)
    {
        totalWaves = StageManager.Instance.GetWavesForCurrentStage();
    }
    StartCoroutine(StartNextWave());
}

    IEnumerator StartNextWave()
    {
        yield return new WaitForSeconds(2f);

        while (currentWave < totalWaves)
        {
            currentWave++;
            Debug.Log($"=== DALGA {currentWave} BAŞLIYOR ===");

            yield return StartCoroutine(SpawnWave(currentWave));

            yield return new WaitUntil(() => enemiesAlive <= 0);
            Debug.Log($"Dalga {currentWave} tamamlandı!");

            if (currentWave < totalWaves)
            {
                Debug.Log($"{timeBetweenWaves} saniye sonra sonraki dalga...");
                yield return new WaitForSeconds(timeBetweenWaves);
            }
        }

        yield return new WaitForSeconds(timeBetweenWaves);
        StartCoroutine(SpawnBoss());
    }

    IEnumerator SpawnWave(int waveNumber)
    {
        waveInProgress = true;

        int enemyCount = baseEnemyCount + (waveNumber - 1) * 2;
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
    GameObject enemy = Instantiate(normalEnemyPrefab, spawnPoint.position, Quaternion.identity);

    EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
    if (enemyStats != null)
    {
        
        float hpMultiplier = Mathf.Pow(enemyHPMultiplier, waveNumber - 1);
        float regionHP = StageManager.Instance != null ? StageManager.Instance.GetRegionHPMultiplier() : 1f;
        enemyStats.maxHP = Mathf.RoundToInt(enemyStats.maxHP * hpMultiplier * regionHP);
        enemyStats.currentHP = enemyStats.maxHP;

        float speedMultiplier = Mathf.Pow(enemySpeedMultiplier, waveNumber - 1);
        float regionSpeed = StageManager.Instance != null ? StageManager.Instance.GetRegionSpeedMultiplier() : 1f;
        enemyStats.moveSpeed *= speedMultiplier * regionSpeed;

        enemyStats.xpReward = Mathf.RoundToInt(enemyStats.xpReward * (1 + (waveNumber - 1) * 0.2f));
    }
}

   IEnumerator SpawnBoss()
    {
    bossSpawned = true;
    Debug.Log("!!! BOSS DALGA BAŞLIYOR !!!");

    
    UIManager uiManager = FindFirstObjectByType<UIManager>();
    if (uiManager != null)
    {
        uiManager.ShowBossWarning();
    }

    
    yield return new WaitForSeconds(2f);

    
    GameObject boss = Instantiate(bossPrefab, rightSpawnPoint.position, Quaternion.identity);
    enemiesAlive = 1;
    }

    public void OnEnemyDied()
    {
        enemiesAlive--;
        Debug.Log($"Düşman öldü! Kalan: {enemiesAlive}");

        if (enemiesAlive <= 0 && bossSpawned)
        {
            StageClear();
        }
    }

void StageClear()
{
    Debug.Log("★ STAGE CLEAR! ★");

    UIManager uiManager = FindFirstObjectByType<UIManager>();
    if (uiManager != null)
    {
        uiManager.ShowStageClear();
    }

    if (StageManager.Instance != null)
    {
        StageManager.Instance.NextStage();
    }

    StartCoroutine(LoadNextStage());
}

IEnumerator LoadNextStage()
{
    yield return new WaitForSeconds(3f);

    UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
}

    public int GetCurrentWave() => currentWave;
    public int GetTotalWaves() => totalWaves;
    public bool IsWaveInProgress() => waveInProgress;
}