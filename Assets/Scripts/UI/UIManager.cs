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
    public TextMeshProUGUI stageInfoText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI hpValueText;
    public TextMeshProUGUI xpValueText;

    [Header("Paneller")]
    public GameObject stageClearPanel;
    public GameObject gameOverPanel;

    [Header("Referanslar")]
    public PlayerStats playerStats;
    public WaveManager waveManager;

    [Header("Boss UI")]
    public GameObject bossHPPanel;
    public Slider bossHPBar;
    public TextMeshProUGUI bossNameText;
    public GameObject bossWarningPanel;

   void Update()
    {
    UpdateHP();
    UpdateXP();
    UpdateWaveText();
    UpdateLevelText();
    UpdateStageInfo();
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

    void UpdateStageInfo()
    {
    if (stageInfoText == null) return;
    if (StageManager.Instance != null)
    {
        stageInfoText.text = StageManager.Instance.GetStageInfo();
    }
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
    {
        bossHPBar.value = currentHP;
    }
}

public void HideBossHP()
{
    if (bossHPPanel != null)
    {
        bossHPPanel.SetActive(false);
    }
}

public void ShowBossWarning()
{
    if (bossWarningPanel != null)
    {
        StartCoroutine(BossWarningCoroutine());
    }
}

IEnumerator BossWarningCoroutine()
{
    bossWarningPanel.SetActive(true);
    yield return new WaitForSeconds(2f);
    bossWarningPanel.SetActive(false);
}

}