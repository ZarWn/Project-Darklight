using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    [Header("Bölge ve Stage Ayarları")]
    public int currentRegion = 1;
    public int currentStage = 1;
    public int totalRegions = 3;
    public int stagesPerRegion = 3;

    [Header("Bölge Güçlendirme Çarpanları")]
    public float regionHPMultiplier = 1.5f;
    public float regionSpeedMultiplier = 1.2f;
    public float regionDamageMultiplier = 1.3f;

    [Header("Stage Ayarları")]
    public int[] wavesPerStage = { 5, 6, 7 };

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
    }

    public int GetWavesForCurrentStage()
    {
        int stageIndex = currentStage - 1;
        if (stageIndex < wavesPerStage.Length)
            return wavesPerStage[stageIndex];
        return 5;
    }

    public float GetRegionHPMultiplier()
    {
        return Mathf.Pow(regionHPMultiplier, currentRegion - 1);
    }

    public float GetRegionSpeedMultiplier()
    {
        return Mathf.Pow(regionSpeedMultiplier, currentRegion - 1);
    }

    public float GetRegionDamageMultiplier()
    {
        return Mathf.Pow(regionDamageMultiplier, currentRegion - 1);
    }

    public void NextStage()
    {
        if (currentStage < stagesPerRegion)
        {
            currentStage++;
            Debug.Log($"Sonraki Stage: Bölge {currentRegion} - Stage {currentStage}");
        }
        else
        {
            NextRegion();
        }
    }

    void NextRegion()
    {
        if (currentRegion < totalRegions)
        {
            currentRegion++;
            currentStage = 1;
            Debug.Log($"Yeni Bölge: Bölge {currentRegion} - Stage {currentStage}");
        }
        else
        {
            GameCompleted();
        }
    }

    void GameCompleted()
    {
        Debug.Log("★ OYUN TAMAMLANDI! ★");
    }

    public string GetRegionName()
    {
        switch (currentRegion)
        {
            case 1: return "Orman";
            case 2: return "Çöl";
            case 3: return "Karanlık Kale";
            default: return "Bilinmeyen";
        }
    }

    public string GetStageInfo()
    {
        return $"Bölge {currentRegion} ({GetRegionName()}) - Stage {currentStage}";
    }
}