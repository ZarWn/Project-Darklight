using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    [Header("Ayarlar")]
    public Transform leftSpawn, rightSpawn;
    public GameObject normalEnemy, eliteEnemy, bossPrefab;
    public float timeBetweenWaves = 3f, spawnInterval = 0.8f;
    public int baseCount = 2; 

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

        int expectedLevel = currentFloor * 2; 
        int levelGap = Mathf.Max(0, playerLevel - expectedLevel); 
        
        int count = baseCount + (waveNum / 2) + (currentFloor / 2);
        int overLevelBonus = levelGap / 3; 
        count += overLevelBonus;
        count = Mathf.Clamp(count, 1, 10); 
        
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

        // KUSURSUZ DENGE: Can artış hızı %10'a sabitlendi. Hasar artışı %12 oldu.
        // Sonuç: Düşmanlar sert vurur (Zorluk) ama hızlı ölürler (Akıcılık).
        float baseHpMult = 1f + ((currentFloor - 1) * 0.10f); 
        float baseDmgMult = 1f + ((currentFloor - 1) * 0.12f);
        float baseSpdMult = 1f + ((currentFloor - 1) * 0.02f);

        // Adaptif sistem oyuncuyu çok cezalandırmayacak şekilde kısıldı.
        float adaptiveHpMult = 1f + (levelGap * 0.03f);
        float adaptiveDmgMult = 1f + (levelGap * 0.02f);

        stats.maxHP = Mathf.RoundToInt(stats.maxHP * baseHpMult * adaptiveHpMult);
        stats.currentHP = stats.maxHP;
        
        int calculatedDamage = Mathf.RoundToInt(stats.damage * baseDmgMult * adaptiveDmgMult);
        stats.damage = Mathf.Max(1, calculatedDamage); 
        
        stats.moveSpeed *= baseSpdMult;
        stats.xpReward = Mathf.RoundToInt(stats.xpReward * (1f + ((currentFloor - 1) * 0.15f))); // Ödül artırıldı
    }

    void SpawnElite()
    {
        GameObject elite = Instantiate(eliteEnemy ? eliteEnemy : normalEnemy, rightSpawn.position, Quaternion.identity);
        elite.layer = LayerMask.NameToLayer("Enemy");

        EnemyStats stats = elite.GetComponent<EnemyStats>();
        EnemyController ec = elite.GetComponent<EnemyController>();
        
        int currentFloor = FloorManager.Instance ? FloorManager.Instance.currentFloor : 1;
        int playerLevel = PlayerStats.Instance != null ? PlayerStats.Instance.currentLevel : 1;
        int levelGap = Mathf.Max(0, playerLevel - (currentFloor * 2));

        if (stats != null)
        {
            // Elitlerin canı aşırı şişmesin diye %15 artışa çekildi.
            float baseHpMult = 1f + ((currentFloor - 1) * 0.15f);
            float adaptiveHpMult = 1f + (levelGap * 0.05f); 
            float baseDmgMult = 1f + ((currentFloor - 1) * 0.12f);
            float adaptiveDmgMult = 1f + (levelGap * 0.03f);
            
            stats.maxHP = Mathf.RoundToInt(stats.maxHP * 2.5f * baseHpMult * adaptiveHpMult); 
            stats.currentHP = stats.maxHP;
            
            int calculatedDamage = Mathf.RoundToInt(stats.damage * 1.5f * baseDmgMult * adaptiveDmgMult);
            stats.damage = Mathf.Max(3, calculatedDamage);
            
            stats.moveSpeed *= 1.10f; 
            stats.xpReward *= 4; 
            
            elite.transform.localScale = new Vector3(elite.transform.localScale.x * 1.5f, elite.transform.localScale.y * 1.5f, elite.transform.localScale.z);
            
            if (ec != null) 
            {
                ec.stopDistance = 2.0f;
                ec.attackDistance = 3.0f; 
            }

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
        int levelGap = Mathf.Max(0, playerLevel - (currentFloor * 2));
        
        if(stats != null)
        {
            // Bossların can artışı yavaşlatıldı ama hasarları hala çok tehlikeli!
            float baseHpMult = 1f + ((currentFloor - 1) * 0.20f);
            float adaptiveHpMult = 1f + (levelGap * 0.05f);
            float baseDmgMult = 1f + ((currentFloor - 1) * 0.15f);
            float adaptiveDmgMult = 1f + (levelGap * 0.04f);
            
            stats.maxHP = Mathf.RoundToInt(stats.maxHP * 4.5f * baseHpMult * adaptiveHpMult);
            stats.currentHP = stats.maxHP;
            
            int calculatedDamage = Mathf.RoundToInt(stats.damage * 2f * baseDmgMult * adaptiveDmgMult);
            stats.damage = Mathf.Max(5, calculatedDamage);
            
            stats.xpReward *= 10;
        }

        boss.transform.localScale = new Vector3(boss.transform.localScale.x * 2f, boss.transform.localScale.y * 2f, boss.transform.localScale.z);

        if (ec != null) 
        {
            ec.stopDistance = 2.8f;
            ec.attackDistance = 4.0f;
        }

        enemiesAlive = 1;
    }

    public void OnEnemyDied() => enemiesAlive--;
    
    IEnumerator FloorCompleteRoutine()
    {
        UIManager ui = FindFirstObjectByType<UIManager>();
        if (ui) ui.ShowStageClear(); 
        if (PlayerStats.Instance != null) PlayerStats.Instance.HealHP(Mathf.RoundToInt(PlayerStats.Instance.maxHP * 0.10f));
        yield return new WaitForSeconds(2.5f); 
        if (FloorManager.Instance) FloorManager.Instance.OnFloorCompleted(); 
    }
}