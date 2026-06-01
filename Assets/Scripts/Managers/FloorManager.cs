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
    public int mapSeed; 
    public int currentNodeIndex = -1; 
    
    public FloorType currentSelectedFloorType = FloorType.Savas; 
    
    private List<List<FloorType>> floorMap = new List<List<FloorType>>();

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }
        
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

    // --- ALTIN ORAN: KUSURSUZ HARİTA ALGORİTMASI ---
    List<FloorType> GenerateFloorOptions(int floorIndex)
    {
        List<FloorType> options = new List<FloorType>();
        int roomCount = (floorIndex % 2 == 0) ? 2 : 3;

        // 1. KURAL: İlk iki kat her zaman Savaş olmalı (Isınma ve altın kasma)
        if (floorIndex < 2)
        {
            for (int i = 0; i < roomCount; i++) options.Add(FloorType.Savas);
            return options;
        }

        // 2. KURAL: Oyunun tam ortası (totalFloors'un yarısı) kesinlikle Hazine olmalı!
        if (floorIndex == (totalFloors / 2) - 1)
        {
            for (int i = 0; i < roomCount; i++) options.Add(FloorType.Hazine);
            return options;
        }

        // 3. KURAL: Final Boss'tan bir önceki kat (Sondan 2. kat) KESİNLİKLE Dinlenme olmalı!
        if (floorIndex == totalFloors - 2)
        {
            for (int i = 0; i < roomCount; i++) options.Add(FloorType.Dinlenme);
            return options;
        }

        // 4. KURAL: Kontrollü ve Dengeli Rastgelelik
        bool hasElite = false;
        bool hasShop = false;
        bool hasRest = false;

        for (int i = 0; i < roomCount; i++)
        {
            float rand = Random.value;

            // Kat 4 ve 10 civarı Market (Shop) çıkma ihtimali aşırı yüksektir
            if ((floorIndex == 4 || floorIndex == 10) && !hasShop && rand > 0.2f)
            {
                options.Add(FloorType.Shop);
                hasShop = true;
            }
            // Elitler 3. Kattan sonra başlar ve bir katta maksimum 1 tane olabilir
            else if (floorIndex > 2 && !hasElite && rand > 0.65f)
            {
                options.Add(FloorType.Elite);
                hasElite = true;
            }
            // Ekstra rastgele Shop veya Dinlenme çıkma ihtimali (Oyun zorlaştıkça nefes aldırır)
            else if (rand > 0.85f)
            {
                if (!hasRest && rand > 0.90f) { options.Add(FloorType.Dinlenme); hasRest = true; }
                else if (!hasShop && rand > 0.85f) { options.Add(FloorType.Shop); hasShop = true; }
                else options.Add(FloorType.Savas);
            }
            else
            {
                options.Add(FloorType.Savas); // Kalan her şey normal savaş
            }
        }

        // Eğer bir katta ilerlenebilecek HİÇBİR normal savaş kalmadıysa, en az 1 tanesini savaşa çevir.
        // (Oyuncu mecbur kalıp sürekli Elit'e girmek zorunda hissetmesin)
        if (!options.Contains(FloorType.Savas) && !options.Contains(FloorType.Elite))
        {
            options[0] = FloorType.Savas;
        }

        // Seçenekleri karıştır ki Market veya Elit hep aynı sütunda (Örn: hep en sağda) çıkmasın
        for (int i = 0; i < options.Count; i++)
        {
            FloorType temp = options[i];
            int randomIndex = Random.Range(i, options.Count);
            options[i] = options[randomIndex];
            options[randomIndex] = temp;
        }

        return options;
    }

    public List<FloorType> GetFloorOptions(int floorIndex) => floorIndex < floorMap.Count ? floorMap[floorIndex] : new List<FloorType> { FloorType.FinalBoss };

    public void SelectFloor(FloorType floorType, int nodeIndex = -1)
    {
        currentSelectedFloorType = floorType; 
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
    public bool IsCurrentFloorBoss() => currentSelectedFloorType == FloorType.Boss || currentSelectedFloorType == FloorType.FinalBoss;
    public bool IsCurrentFloorElite() => currentSelectedFloorType == FloorType.Elite;

    public int GetEnemyCountBonus() => currentFloor / 2; 
    public float GetEnemyHPMultiplier() => 1f + (currentFloor * 0.15f); 
    public float GetEnemySpeedMultiplier() => 1f + (currentFloor * 0.05f); 

    public void OnFloorCompleted()
    {
        SceneManager.LoadScene("FloorSelectScene"); 
    }
}