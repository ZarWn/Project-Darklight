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

            yield return new WaitUntil(() => enemiesAlive <= 0);
            
            if (currentWave < totalWaves) yield return new WaitForSeconds(timeBetweenWaves);
        }
        
        StartCoroutine(FloorCompleteRoutine());
    }

    IEnumerator SpawnWave(int waveNum)
    {
        int currentFloor = FloorManager.Instance ? FloorManager.Instance.currentFloor : 1;
        int playerLevel = PlayerStats.Instance != null ? PlayerStats.Instance.currentLevel : 1;

        int expectedLevel = currentFloor * 3;
        int levelGap = Mathf.Max(0, playerLevel - expectedLevel); 
        
        int floorBonus = FloorManager.Instance ? FloorManager.Instance.GetEnemyCountBonus() : 0;
        
        int count = baseCount + waveNum + (currentFloor - 1) + floorBonus;
        
        int overLevelBonus = Mathf.Min(3, levelGap / 2);
        count += overLevelBonus;

        count = Mathf.Clamp(count, 1, 15);
        
        enemiesAlive = count;

        for (int i = 0; i < count; i++)
        {
            Transform sp = (i % 2 == 0) ? rightSpawn : leftSpawn;
            SpawnEnemy(sp.position, waveNum, currentFloor, levelGap);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemy(Vector3 pos, int waveNum, int currentFloor, int levelGap)
    {
        GameObject enemy = Instantiate(normalEnemy, pos, Quaternion.identity);
        enemy.layer = LayerMask.NameToLayer("Enemy"); 
        
        EnemyStats stats = enemy.GetComponent<EnemyStats>();
        if (stats == null) return;

        float baseHpMult = 1f + (currentFloor * 0.25f) + (waveNum * 0.1f);
        float baseDmgMult = 1f + (currentFloor * 0.15f) + (waveNum * 0.05f);
        float baseSpdMult = 1f + (currentFloor * 0.05f);

        float adaptiveHpMult = 1f + (levelGap * 0.08f);
        float adaptiveDmgMult = 1f + (levelGap * 0.04f);

        stats.maxHP = Mathf.RoundToInt(stats.maxHP * baseHpMult * adaptiveHpMult);
        stats.currentHP = stats.maxHP;
        
        int calculatedDamage = Mathf.RoundToInt(stats.damage * baseDmgMult * adaptiveDmgMult);
        stats.damage = Mathf.Max(5, calculatedDamage); 
        
        stats.moveSpeed *= baseSpdMult;
        stats.xpReward = Mathf.RoundToInt(stats.xpReward * (1 + currentFloor * 0.1f));
    }

    void SpawnElite()
    {
        GameObject elite = Instantiate(eliteEnemy ? eliteEnemy : normalEnemy, rightSpawn.position, Quaternion.identity);
        elite.layer = LayerMask.NameToLayer("Enemy");

        EnemyStats stats = elite.GetComponent<EnemyStats>();
        EnemyController ec = elite.GetComponent<EnemyController>();
        
        int currentFloor = FloorManager.Instance ? FloorManager.Instance.currentFloor : 1;
        int playerLevel = PlayerStats.Instance != null ? PlayerStats.Instance.currentLevel : 1;
        int levelGap = Mathf.Max(0, playerLevel - (currentFloor * 3));

        if (stats != null)
        {
            float baseHpMult = 1f + (currentFloor * 0.30f);
            float adaptiveHpMult = 1f + (levelGap * 0.10f); 
            
            stats.maxHP = Mathf.RoundToInt(stats.maxHP * 4f * baseHpMult * adaptiveHpMult); 
            stats.currentHP = stats.maxHP;
            
            int calculatedDamage = Mathf.RoundToInt(stats.damage * 2f * (1f + (currentFloor * 0.2f)) * (1f + (levelGap * 0.05f)));
            stats.damage = Mathf.Max(10, calculatedDamage);
            
            stats.moveSpeed *= 1.15f; 
            stats.xpReward *= 5; 
            
            elite.transform.localScale = new Vector3(
                elite.transform.localScale.x * 1.5f,
                elite.transform.localScale.y * 1.5f,
                elite.transform.localScale.z
            );
            
            // --- HATA ÇÖZÜMÜ ---
            // Çarpmak yerine Elit'in durma mesafesini sabitledik. En kısa kılıç bile rahatça vurabilir.
            if (ec != null) ec.stopDistance = 1.5f;

            SpriteRenderer sr = elite.GetComponentInChildren<SpriteRenderer>();
            if (sr != null) sr.color = new Color(1f, 0.4f, 0.4f);
        }
        enemiesAlive = 1;
    }

    IEnumerator SpawnBoss()
    {
        UIManager ui = FindFirstObjectByType<UIManager>();
        if (ui) ui.ShowBossWarning();
        
        yield return new WaitForSeconds(2f);
        GameObject boss = Instantiate(bossPrefab, rightSpawn.position, Quaternion.identity);
        boss.layer = LayerMask.NameToLayer("Enemy");
        
        EnemyStats stats = boss.GetComponent<EnemyStats>();
        EnemyController ec = boss.GetComponent<EnemyController>();
        
        int currentFloor = FloorManager.Instance ? FloorManager.Instance.currentFloor : 1;
        int playerLevel = PlayerStats.Instance != null ? PlayerStats.Instance.currentLevel : 1;
        int levelGap = Mathf.Max(0, playerLevel - (currentFloor * 3));
        
        if(stats != null)
        {
            float baseHpMult = 1f + (currentFloor * 0.40f);
            float adaptiveHpMult = 1f + (levelGap * 0.12f);
            
            stats.maxHP = Mathf.RoundToInt(stats.maxHP * 10f * baseHpMult * adaptiveHpMult);
            stats.currentHP = stats.maxHP;
            
            int calculatedDamage = Mathf.RoundToInt(stats.damage * 3.5f * (1f + (currentFloor * 0.25f)) * (1f + (levelGap * 0.08f)));
            stats.damage = Mathf.Max(15, calculatedDamage);
            
            stats.xpReward *= 15;
        }

        boss.transform.localScale = new Vector3(
            boss.transform.localScale.x * 2f, 
            boss.transform.localScale.y * 2f,
            boss.transform.localScale.z
        );

        // --- HATA ÇÖZÜMÜ ---
        // Boss 2 kat büyük olsa da dibimize kadar girmek zorunda kalacak.
        if (ec != null) ec.stopDistance = 1.8f;

        enemiesAlive = 1;
    }

    public void OnEnemyDied() => enemiesAlive--;

    IEnumerator FloorCompleteRoutine()
    {
        UIManager ui = FindFirstObjectByType<UIManager>();
        if (ui) ui.ShowStageClear(); 
        
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.HealHP(Mathf.RoundToInt(PlayerStats.Instance.maxHP * 0.10f));
        }

        yield return new WaitForSeconds(2.5f); 
        
        if (FloorManager.Instance) FloorManager.Instance.OnFloorCompleted(); 
    }
}