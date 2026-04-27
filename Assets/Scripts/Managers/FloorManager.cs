using UnityEngine;
using System.Collections.Generic;

public class FloorManager : MonoBehaviour
{
    public static FloorManager Instance;

    [Header("Kat Ayarları")]
    public int currentFloor = 0;
    public int totalFloors = 16;

    public enum FloorType
    {
        Savas,
        Elite,
        Shop,
        Hazine,
        Dinlenme,
        Boss,
        FinalBoss
    }

    private List<int> bossFloors = new List<int> { 15 }; // Sadece son kat boss
    private int finalBossFloor = 15;

    private int[] optionsPerFloor = {
        2, // Kat 1
        2, // Kat 2
        3, // Kat 3
        2, // Kat 4
        3, // Kat 5
        2, // Kat 6
        3, // Kat 7
        2, // Kat 8
        3, // Kat 9
        2, // Kat 10
        3, // Kat 11
        2, // Kat 12
        3, // Kat 13
        2, // Kat 14
        3, // Kat 15
        1, // Kat 16 - FINAL BOSS
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

        Debug.Log("Harita olusturuldu!");
    }

    List<FloorType> GenerateRandomOptions(int count, int floorIndex)
{
    List<FloorType> options = new List<FloorType>();
    List<FloorType> lastFloorTypes = new List<FloorType>();

    if (floorIndex > 0 && floorMap.Count > floorIndex - 1)
        lastFloorTypes = floorMap[floorIndex - 1];

    List<FloorType> weightedTypes = new List<FloorType>();

    // İlk 2 katta sadece savaş olsun
    if (floorIndex < 2)
    {
        for (int i = 0; i < count; i++)
            options.Add(FloorType.Savas);
        return options;
    }

    // Savaş her zaman çıkabilir
    weightedTypes.Add(FloorType.Savas);
    weightedTypes.Add(FloorType.Savas); // Savaş daha sık çıksın

    // Elite sadece 4. kattan sonra
    if (floorIndex >= 3)
        weightedTypes.Add(FloorType.Elite);

    // Shop her 4 katta bir çıkabilir
    if (floorIndex % 4 == 0)
        weightedTypes.Add(FloorType.Shop);

    // Hazine her 5 katta bir çıkabilir
    if (floorIndex % 5 == 0)
        weightedTypes.Add(FloorType.Hazine);

    // Dinlenme her 4 katta bir çıkabilir
    if (floorIndex % 4 == 2)
        weightedTypes.Add(FloorType.Dinlenme);

    // Liste yetmezse savaş ekle
    while (weightedTypes.Count < count)
        weightedTypes.Add(FloorType.Savas);

    List<FloorType> usedTypes = new List<FloorType>();

    for (int i = 0; i < count; i++)
    {
        List<FloorType> available = new List<FloorType>();

        foreach (FloorType type in weightedTypes)
        {
            if (!usedTypes.Contains(type) && !lastFloorTypes.Contains(type))
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

    public List<FloorType> GetCurrentFloorOptions()
    {
        return GetFloorOptions(currentFloor);
    }

    public void SelectFloor(FloorType floorType)
    {
        currentFloor++;
        Debug.Log($"Kat {currentFloor} secildi: {floorType}");
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

    public bool IsCurrentFloorElite()
    {
        var options = GetCurrentFloorOptions();
        return options.Count == 1 && options[0] == FloorType.Elite;
    }

    public bool IsCurrentFloorBoss()
    {
        var options = GetCurrentFloorOptions();
        return options.Count == 1 &&
               (options[0] == FloorType.Boss || options[0] == FloorType.FinalBoss);
    }

    public int GetWavesForCurrentFloor()
    {
        if (currentFloor <= 3) return 3;
        if (currentFloor <= 6) return 4;
        if (currentFloor <= 10) return 5;
        return 6;
    }

    public float GetEnemyHPMultiplier()
    {
        return 1f + (currentFloor * 0.15f);
    }

    public float GetEnemySpeedMultiplier()
    {
        return 1f + (currentFloor * 0.05f);
    }

    public int GetEnemyCountBonus()
    {
        return currentFloor / 3;
    }

    public string GetFloorTypeName(FloorType floorType)
    {
        switch (floorType)
        {
            case FloorType.Savas: return "Savas";
            case FloorType.Elite: return "Elite";
            case FloorType.Shop: return "Market";
            case FloorType.Hazine: return "Hazine";
            case FloorType.Dinlenme: return "Dinlenme";
            case FloorType.Boss: return "Boss";
            case FloorType.FinalBoss: return "Final Boss";
            default: return "Bilinmeyen";
        }
    }
}