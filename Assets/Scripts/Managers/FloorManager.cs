using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class FloorManager : MonoBehaviour
{
    public static FloorManager Instance;
    public enum FloorType { Savas, Elite, Shop, Hazine, Dinlenme, Boss, FinalBoss }

    [Header("Kat Ayarları")]
    public int currentFloor = 0;
    public int totalFloors = 16;
    
    // --- HATA ÇÖZÜMÜ: Haritanın yolları unutmaması için "Harita Şifresi" ---
    public int mapSeed; 
    
    public int currentNodeIndex = -1; 
    
    private List<List<FloorType>> floorMap = new List<List<FloorType>>();

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }
        
        // Oyun ilk başladığında eşsiz bir harita şifresi üretip kilitliyoruz.
        mapSeed = Random.Range(10000, 999999);
        Random.InitState(mapSeed); 
        
        GenerateRandomMap();
    }

    void GenerateRandomMap()
    {
        floorMap.Clear();
        for (int i = 0; i < totalFloors; i++)
        {
            if (i == totalFloors - 1) floorMap.Add(new List<FloorType> { FloorType.FinalBoss }); 
            else floorMap.Add(GenerateFloorOptions(i));
        }
    }

    List<FloorType> GenerateFloorOptions(int floorIndex)
    {
        List<FloorType> options = new List<FloorType>();
        int roomCount = (floorIndex % 2 == 0) ? 2 : 3;

        for (int i = 0; i < roomCount; i++)
        {
            if (floorIndex < 2) options.Add(FloorType.Savas);
            else if (Random.value > 0.7f && floorIndex > 3) options.Add(FloorType.Elite);
            else if (Random.value > 0.8f) options.Add((FloorType)Random.Range(2, 5));
            else options.Add(FloorType.Savas);
        }
        return options;
    }

    public List<FloorType> GetFloorOptions(int floorIndex) => floorIndex < floorMap.Count ? floorMap[floorIndex] : new List<FloorType> { FloorType.FinalBoss };

    public void SelectFloor(FloorType floorType, int nodeIndex = -1)
    {
        currentFloor++;
        currentNodeIndex = nodeIndex; 
        
        string sceneName = "GameScene";
        if (floorType == FloorType.Shop) sceneName = "ShopScene";
        else if (floorType == FloorType.Hazine) sceneName = "TreasureScene";
        else if (floorType == FloorType.Dinlenme) sceneName = "RestScene";
        
        SceneManager.LoadScene(sceneName);
    }

    public int GetTotalFloors() => totalFloors;

    public int GetWavesForCurrentFloor() => 3 + (currentFloor / 3); 
    
    public bool IsCurrentFloorBoss() 
    {
        var options = GetFloorOptions(currentFloor);
        return options.Contains(FloorType.Boss) || options.Contains(FloorType.FinalBoss);
    }
    
    public bool IsCurrentFloorElite()
    {
        var options = GetFloorOptions(currentFloor);
        return options.Contains(FloorType.Elite);
    }

    public int GetEnemyCountBonus() => currentFloor / 2; 
    public float GetEnemyHPMultiplier() => 1f + (currentFloor * 0.15f); 
    public float GetEnemySpeedMultiplier() => 1f + (currentFloor * 0.05f); 

    public void OnFloorCompleted()
    {
        SceneManager.LoadScene("FloorSelectScene"); 
    }
}