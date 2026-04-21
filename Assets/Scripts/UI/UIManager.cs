using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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
    public TextMeshProUGUI floorText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI killText;

    [Header("Paneller")]
    public GameObject stageClearPanel;
    public GameObject gameOverPanel;

    [Header("Boss UI")]
    public GameObject bossHPPanel;
    public Slider bossHPBar;
    public TextMeshProUGUI bossNameText;
    public GameObject bossWarningPanel;

    [Header("Referanslar")]
    public PlayerStats playerStats;
    public WaveManager waveManager;

    void Update()
    {
        UpdateHP();
        UpdateXP();
        UpdateWaveText();
        UpdateLevelText();
        UpdateGoldText();
        UpdateKillText();
        UpdateFloorText();
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
        if (waveText != null)
            waveText.text = $"Dalga: {waveManager.GetCurrentWave()}/{waveManager.GetTotalWaves()}";
    }

    void UpdateLevelText()
    {
        if (playerStats == null) return;
        if (levelText != null)
            levelText.text = $"Seviye: {playerStats.currentLevel}";
    }

    void UpdateGoldText()
    {
        if (playerStats == null) return;
        if (goldText != null)
            goldText.text = $"Altin: {playerStats.gold}";
    }

    void UpdateKillText()
    {
        if (playerStats == null) return;
        if (killText != null)
            killText.text = $"Kill: {playerStats.killCount}";
    }

    void UpdateFloorText()
    {
        if (floorText != null && FloorManager.Instance != null)
            floorText.text = $"Kat: {FloorManager.Instance.currentFloor}/15";
    }

    public void ShowStageClear()
    {
        if (stageClearPanel != null)
            stageClearPanel.SetActive(true);
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void ShowBossHP(int maxHP, string bossName)
    {
        if (bossHPPanel != null)
        {
            bossHPPanel.SetActive(true);
            bossHPBar.maxValue = maxHP;
            bossHPBar.value = maxHP;
            bossNameText.text = bossName;
        }
    }

    public void UpdateBossHP(int currentHP)
    {
        if (bossHPBar != null)
            bossHPBar.value = currentHP;
    }

    public void HideBossHP()
    {
        if (bossHPPanel != null)
            bossHPPanel.SetActive(false);
    }

    public void ShowBossWarning()
    {
        if (bossWarningPanel != null)
            StartCoroutine(BossWarningCoroutine());
    }

    IEnumerator BossWarningCoroutine()
    {
        bossWarningPanel.SetActive(true);
        yield return new WaitForSeconds(2f);
        bossWarningPanel.SetActive(false);
    }
}