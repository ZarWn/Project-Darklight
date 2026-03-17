using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Barlar")]
    public Slider hpBar;
    public Slider xpBar;

    [Header("Textler")]
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI hpValueText;
    public TextMeshProUGUI xpValueText;

    [Header("Paneller")]
    public GameObject stageClearPanel;
    public GameObject gameOverPanel;

    [Header("Referanslar")]
    public PlayerStats playerStats;
    public WaveManager waveManager;

    void Update()
    {
        UpdateHP();
        UpdateXP();
        UpdateWaveText();
        UpdateLevelText();
    }

    void UpdateHP()
    {
        if (playerStats == null) return;
        hpBar.value = playerStats.currentHP;
        hpBar.maxValue = playerStats.maxHP;

        if (hpValueText != null)
            hpValueText.text = $"{playerStats.currentHP}/{playerStats.maxHP}";
    }

    void UpdateXP()
    {
        if (playerStats == null) return;
        xpBar.value = playerStats.currentXP;
        xpBar.maxValue = playerStats.xpToNextLevel;

        if (xpValueText != null)
            xpValueText.text = $"{playerStats.currentXP}/{playerStats.xpToNextLevel}";
    }

    void UpdateWaveText()
    {
        if (waveManager == null) return;
        waveText.text = $"Dalga: {waveManager.GetCurrentWave()}/{waveManager.GetTotalWaves()}";
    }

    void UpdateLevelText()
    {
        if (playerStats == null) return;
        if (levelText != null)
            levelText.text = $"Seviye: {playerStats.currentLevel}";
    }

    public void ShowStageClear()
    {
        stageClearPanel.SetActive(true);
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
    }
}