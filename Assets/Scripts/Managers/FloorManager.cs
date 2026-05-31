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
    
    // Eski harita tasarımının yolları çizebilmesi için gereken hafıza değişkeni
    public int currentNodeIndex = -1; 
    
    private List<List<FloorType>> floorMap = new List<List<FloorType>>();

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }
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

    // HATA ÇÖZÜMÜ: Harita yöneticisinin beklediği 2 parametreli fonksiyon
    public void SelectFloor(FloorType floorType, int nodeIndex = -1)
    {
        currentFloor++;
        currentNodeIndex = nodeIndex; // Seçilen odanın indeksini kaydet ki sonraki yollar doğru çizilsin
        
        string sceneName = "GameScene";
        if (floorType == FloorType.Shop) sceneName = "ShopScene";
        else if (floorType == FloorType.Hazine) sceneName = "TreasureScene";
        else if (floorType == FloorType.Dinlenme) sceneName = "RestScene";
        
        SceneManager.LoadScene(sceneName);
    }

    // HATA ÇÖZÜMÜ: Haritanın kat sayısını okuduğu fonksiyon
    public int GetTotalFloors() => totalFloors;

    // ---------------------------------------------------------
    // WaveManager ve Zorluk Ayarları
    
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
        Debug.Log($"Kat {currentFloor} tamamlandı!");
        SceneManager.LoadScene("FloorSelectScene"); 
    }
}