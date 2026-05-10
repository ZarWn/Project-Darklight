using UnityEngine;
using System.Collections.Generic;

public class FloorManager : MonoBehaviour
{
    public static FloorManager Instance;

    [Header("Kat Ayarları")]
    public int currentFloor = 0;
    public int totalFloors = 16;
    public int currentNodeIndex = -1;

    public enum FloorType
    {
        Savas, Elite, Shop, Hazine, Dinlenme, Boss, FinalBoss
    }

    private int finalBossFloor = 15;

    private int[] optionsPerFloor = {
        2, 2, 3, 2, 3, 2, 3, 2, 3, 2, 3, 2, 3, 2, 3, 1
    };

    private List<List<FloorType>> floorMap = new List<List<FloorType>>();

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
            return;
        }

        GenerateRandomMap();
    }

    void GenerateRandomMap()
    {
        floorMap.Clear();

        for (int i = 0; i < totalFloors; i++)
        {
            List<FloorType> floorOptions = new List<FloorType>();

            if (i == finalBossFloor)
            {
                floorOptions.Add(FloorType.FinalBoss);
            }
            else
            {
                int optionCount = optionsPerFloor[i];
                floorOptions = GenerateRandomOptions(optionCount, i);
            }

            floorMap.Add(floorOptions);
        }

        Debug.Log("Dengeli ve Rastgele Harita olusturuldu!");
    }

    List<FloorType> GenerateRandomOptions(int count, int floorIndex)
    {
        List<FloorType> options = new List<FloorType>();
        List<FloorType> lastFloorTypes = new List<FloorType>();

        if (floorIndex > 0 && floorMap.Count > floorIndex - 1)
            lastFloorTypes = floorMap[floorIndex - 1];

        // TEMPO KONTROLÜ: Bir önceki katta barışçıl (aksiyonsuz) bir oda var mıydı?
        bool hadPeacefulRoom = lastFloorTypes.Contains(FloorType.Shop) || 
                               lastFloorTypes.Contains(FloorType.Hazine) || 
                               lastFloorTypes.Contains(FloorType.Dinlenme);

        List<FloorType> weightedTypes = new List<FloorType>();

        // İlk 2 katta sadece savaş
        if (floorIndex < 2)
        {
            for (int i = 0; i < count; i++) options.Add(FloorType.Savas);
            return options;
        }

        // Savaş her zaman havuzda olmalı
        weightedTypes.Add(FloorType.Savas);
        weightedTypes.Add(FloorType.Savas);

        // Elite odalar 3. kattan sonra eklenebilir
        if (floorIndex >= 3)
            weightedTypes.Add(FloorType.Elite);

        // EĞER BİR ÖNCEKİ KATTA BARIŞÇIL ODA YOKSA, BUNLARI HAVUZA EKLE:
        if (!hadPeacefulRoom)
        {
            if (floorIndex % 3 == 0) weightedTypes.Add(FloorType.Shop);
            if (floorIndex % 4 == 0) weightedTypes.Add(FloorType.Hazine);
            if (floorIndex % 3 == 2) weightedTypes.Add(FloorType.Dinlenme);
        }

        // Eğer havuz yetersizse savaşla doldur
        while (weightedTypes.Count < count)
            weightedTypes.Add(FloorType.Savas);

        List<FloorType> usedTypes = new List<FloorType>();

        for (int i = 0; i < count; i++)
        {
            List<FloorType> available = new List<FloorType>();

            foreach (FloorType type in weightedTypes)
            {
                // Aynı katta aynı odadan 2 tane olmasını engelle (Çifte market vs olmasın)
                if (!usedTypes.Contains(type))
                    available.Add(type);
            }

            if (available.Count == 0)
                available.Add(FloorType.Savas);

            int randomIndex = Random.Range(0, available.Count);
            FloorType selected = available[randomIndex];
            options.Add(selected);
            usedTypes.Add(selected);
        }

        return options;
    }

    public List<FloorType> GetFloorOptions(int floorIndex)
    {
        if (floorIndex < floorMap.Count)
            return floorMap[floorIndex];
        return new List<FloorType> { FloorType.FinalBoss };
    }

    public List<FloorType> GetCurrentFloorOptions() => GetFloorOptions(currentFloor);

    public void SelectFloor(FloorType floorType, int nodeIndex)
    {
        currentFloor++;
        currentNodeIndex = nodeIndex; 
        Debug.Log($"Kat {currentFloor} secildi: {floorType}, Yol İndeksi: {nodeIndex}");
        LoadFloor(floorType);
    }

    void LoadFloor(FloorType floorType)
    {
        switch (floorType)
        {
            case FloorType.Savas:
            case FloorType.Elite:
            case FloorType.Boss:
            case FloorType.FinalBoss:
                UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
                break;
            case FloorType.Shop:
                UnityEngine.SceneManagement.SceneManager.LoadScene("ShopScene");
                break;
            case FloorType.Hazine:
                UnityEngine.SceneManagement.SceneManager.LoadScene("TreasureScene");
                break;
            case FloorType.Dinlenme:
                UnityEngine.SceneManagement.SceneManager.LoadScene("RestScene");
                break;
        }
    }

    public void OnFloorCompleted()
    {
        Debug.Log($"Kat {currentFloor} tamamlandi!");
        UnityEngine.SceneManagement.SceneManager.LoadScene("FloorSelectScene");
    }

    public int GetTotalFloors() => totalFloors;
    public bool IsCurrentFloorElite() { var opt = GetCurrentFloorOptions(); return opt.Count == 1 && opt[0] == FloorType.Elite; }
    public bool IsCurrentFloorBoss() { var opt = GetCurrentFloorOptions(); return opt.Count == 1 && (opt[0] == FloorType.Boss || opt[0] == FloorType.FinalBoss); }
    public int GetWavesForCurrentFloor() { if (currentFloor <= 3) return 3; if (currentFloor <= 6) return 4; if (currentFloor <= 10) return 5; return 6; }
    public float GetEnemyHPMultiplier() => 1f + (currentFloor * 0.15f);
    public float GetEnemySpeedMultiplier() => 1f + (currentFloor * 0.05f);
    public int GetEnemyCountBonus() => currentFloor / 3;
}