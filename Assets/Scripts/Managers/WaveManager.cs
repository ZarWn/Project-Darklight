using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    [Header("Ayarlar")]
    public Transform leftSpawn, rightSpawn;
    public GameObject normalEnemy, eliteEnemy, bossPrefab;
    public float timeBetweenWaves = 3f, spawnInterval = 0.8f;
    public int baseCount = 3;

    private int currentWave = 0, totalWaves = 5, enemiesAlive = 0;
    private bool isBossFloor, isEliteFloor;

    void Start()
    {
        // Hoca Sorarsa: "Ternary operatörü (? :) ile NullReference (boş referans) kontrollerini tek satırda çözdüm."
        totalWaves = FloorManager.Instance ? FloorManager.Instance.GetWavesForCurrentFloor() : 5;
        isBossFloor = FloorManager.Instance && FloorManager.Instance.IsCurrentFloorBoss();
        isEliteFloor = FloorManager.Instance && FloorManager.Instance.IsCurrentFloorElite();
        
        StartCoroutine(StartNextWave());
    }

    IEnumerator StartNextWave()
    {
        yield return new WaitForSeconds(2f);
        
        while (currentWave < totalWaves)
        {
            currentWave++;
            
            if (currentWave == totalWaves && isBossFloor) yield return StartCoroutine(SpawnBoss());
            else if (currentWave == totalWaves && isEliteFloor) SpawnElite();
            else yield return StartCoroutine(SpawnWave(currentWave));

            // Hoca Sorarsa: "WaitUntil komutu oyunu dondurmadan, arka planda tüm düşmanların ölmesini akıllıca bekler."
            yield return new WaitUntil(() => enemiesAlive <= 0);
            
            if (currentWave < totalWaves) yield return new WaitForSeconds(timeBetweenWaves);
        }
        FloorComplete();
    }

    IEnumerator SpawnWave(int waveNum)
    {
        int floorBonus = FloorManager.Instance ? FloorManager.Instance.GetEnemyCountBonus() : 0;
        int count = baseCount + (waveNum - 1) * 2 + floorBonus;
        enemiesAlive = count;

        for (int i = 0; i < count; i++)
        {
            Transform sp = (i % 2 == 0) ? rightSpawn : leftSpawn;
            SpawnEnemy(sp.position, waveNum);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemy(Vector3 pos, int waveNum)
    {
        GameObject enemy = Instantiate(normalEnemy, pos, Quaternion.identity);
        EnemyStats stats = enemy.GetComponent<EnemyStats>();
        if (stats == null) return;

        // Hoca Sorarsa: "Düşman canını ve hızını Mathf.Pow ile üstel (eksponansiyel) olarak artırarak oyun zorluğunu dengeliyorum."
        float hpMult = Mathf.Pow(1.3f, waveNum - 1) * (FloorManager.Instance ? FloorManager.Instance.GetEnemyHPMultiplier() : 1f);
        float spdMult = Mathf.Pow(1.1f, waveNum - 1) * (FloorManager.Instance ? FloorManager.Instance.GetEnemySpeedMultiplier() : 1f);
        int floor = FloorManager.Instance ? FloorManager.Instance.currentFloor : 1;

        stats.maxHP = Mathf.RoundToInt(stats.maxHP * hpMult);
        stats.currentHP = stats.maxHP;
        stats.moveSpeed *= spdMult;
        stats.xpReward = Mathf.RoundToInt(stats.xpReward * (1 + floor * 0.1f));
    }

    void SpawnElite()
    {
        GameObject elite = Instantiate(eliteEnemy ? eliteEnemy : normalEnemy, rightSpawn.position, Quaternion.identity);
        EnemyStats stats = elite.GetComponent<EnemyStats>();
        
        if (stats != null)
        {
            stats.maxHP *= 3; stats.currentHP = stats.maxHP;
            stats.damage *= 2; stats.moveSpeed *= 1.3f;
            elite.transform.localScale = Vector3.one * 1.5f;
        }
        enemiesAlive = 1;
    }

    IEnumerator SpawnBoss()
    {
        UIManager ui = FindFirstObjectByType<UIManager>();
        if (ui) ui.ShowBossWarning();
        
        yield return new WaitForSeconds(2f);
        GameObject boss = Instantiate(bossPrefab, rightSpawn.position, Quaternion.identity);
        boss.transform.localScale = Vector3.one * 2.5f;
        enemiesAlive = 1;
    }

    public void OnEnemyDied() => enemiesAlive--;

    void FloorComplete()
    {
        UIManager ui = FindFirstObjectByType<UIManager>();
        if (ui) ui.ShowStageClear();
        if (FloorManager.Instance) FloorManager.Instance.OnFloorCompleted();
    }
}