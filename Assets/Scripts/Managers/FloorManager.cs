using UnityEngine;
using System.Collections.Generic;

public class FloorManager : MonoBehaviour
{
    public static FloorManager Instance;

    [Header("Kat Ayarları")]
    public int currentFloor = 0;
    public int totalFloors = 16; // 15 kat + Final Boss

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

    // Boss katları sabit
    private List<int> bossFloors = new List<int> { 6, 9, 12 };  // Kat 7, 10, 13
    private int finalBossFloor = 15; // Kat 16

    // Kat başına kaç seçenek olacak
    private int[] optionsPerFloor = {
        2, // Kat 1
        2, // Kat 2
        3, // Kat 3
        2, // Kat 4
        3, // Kat 5
        2, // Kat 6
        1, // Kat 7 - BOSS (sabit)
        3, // Kat 8
        2, // Kat 9
        1, // Kat 10 - BOSS (sabit)
        3, // Kat 11
        2, // Kat 12
        1, // Kat 13 - BOSS (sabit)
        3, // Kat 14
        2, // Kat 15
        1, // Kat 16 - FINAL BOSS (sabit)
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

            // Boss katları sabit
            if (bossFloors.Contains(i))
            {
                floorOptions.Add(FloorType.Boss);
            }
            else if (i == finalBossFloor)
            {
                floorOptions.Add(FloorType.FinalBoss);
            }
            else
            {
                // Diğer katlar random
                int optionCount = optionsPerFloor[i];
                floorOptions = GenerateRandomOptions(optionCount, i);
            }

            floorMap.Add(floorOptions);
        }

        Debug.Log("Harita oluşturuldu!");
    }

    List<FloorType> GenerateRandomOptions(int count, int floorIndex)
    {
        List<FloorType> options = new List<FloorType>();
        List<FloorType> availableTypes = new List<FloorType>();

        // Erken katlarda elite çıkmasın
        if (floorIndex < 3)
        {
            availableTypes.Add(FloorType.Savas);
            availableTypes.Add(FloorType.Savas);
            availableTypes.Add(FloorType.Hazine);
            availableTypes.Add(FloorType.Shop);
            availableTypes.Add(FloorType.Dinlenme);
        }
        else if (floorIndex < 6)
        {
            availableTypes.Add(FloorType.Savas);
            availableTypes.Add(FloorType.Savas);
            availableTypes.Add(FloorType.Elite);
            availableTypes.Add(FloorType.Hazine);
            availableTypes.Add(FloorType.Shop);
            availableTypes.Add(FloorType.Dinlenme);
        }
        else
        {
            availableTypes.Add(FloorType.Savas);
            availableTypes.Add(FloorType.Elite);
            availableTypes.Add(FloorType.Elite);
            availableTypes.Add(FloorType.Hazine);
            availableTypes.Add(FloorType.Shop);
            availableTypes.Add(FloorType.Dinlenme);
        }

        // Aynı tip iki kez çıkmasın
        List<FloorType> usedTypes = new List<FloorType>();

        for (int i = 0; i < count; i++)
        {
            List<FloorType> remaining = new List<FloorType>();
            foreach (FloorType type in availableTypes)
            {
                if (!usedTypes.Contains(type))
                    remaining.Add(type);
            }

            if (remaining.Count == 0) break;

            int randomIndex = Random.Range(0, remaining.Count);
            FloorType selected = remaining[randomIndex];
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
        Debug.Log($"Kat {currentFloor} seçildi: {floorType}");
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
        Debug.Log($"Kat {currentFloor} tamamlandı!");
        UnityEngine.SceneManagement.SceneManager.LoadScene("FloorSelectScene");
    }

    public int GetTotalFloors() => totalFloors;

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