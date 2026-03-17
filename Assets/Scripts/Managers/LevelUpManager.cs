using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpManager : MonoBehaviour
{
    [Header("Panel")]
    public GameObject levelUpPanel;

    [Header("Kart Başlıkları")]
    public TextMeshProUGUI card1Title;
    public TextMeshProUGUI card2Title;
    public TextMeshProUGUI card3Title;

    [Header("Kart Açıklamaları")]
    public TextMeshProUGUI card1Desc;
    public TextMeshProUGUI card2Desc;
    public TextMeshProUGUI card3Desc;

    [Header("Kart İkonları")]
    public TextMeshProUGUI card1Icon;
    public TextMeshProUGUI card2Icon;
    public TextMeshProUGUI card3Icon;

    [Header("Referanslar")]
    public PlayerController playerController;
    public PlayerStats playerStats;

    private string[] abilityNames = {
        "Saldırı Hasarı",
        "Saldırı Hızı",
        "Saldırı Menzili",
        "Can Yenileme",
        "Çift Hasar",
        "Zırh",
        "Ateş Hasarı",
        "Max Can Artışı",
        "Güçlü Darbe",
        "Süper Hız"
    };

    private string[] abilityDescs = {
        "Saldırı hasarın\n+5 artar",
        "Saldırı hızın\nönemli ölçüde artar",
        "Saldırı menzili\n+0.5 artar",
        "10 can\nyenilenir",
        "Saldırı hasarın\n2 katına çıkar",
        "Düşman hasarını\n2 azaltır",
        "Her vuruşta\nekstra ateş hasarı",
        "Maximum can\n+20 artar",
        "Saldırı hasarın\n+10 artar",
        "Saldırı hızın\nmaksimuma çıkar"
    };

    private string[] abilityIcons = {
        "⚔️", "⚡", "📏", "❤️", "💥",
        "🛡️", "🔥", "💗", "🗡️", "🌪️"
    };

    private int ability1Index;
    private int ability2Index;
    private int ability3Index;

    public void ShowLevelUpPanel()
    {
        Time.timeScale = 0f;
        SelectRandomAbilities();
        levelUpPanel.SetActive(true);
    }

    void SelectRandomAbilities()
    {
        ability1Index = Random.Range(0, abilityNames.Length);

        do
        {
            ability2Index = Random.Range(0, abilityNames.Length);
        }
        while (ability2Index == ability1Index);

        do
        {
            ability3Index = Random.Range(0, abilityNames.Length);
        }
        while (ability3Index == ability1Index || ability3Index == ability2Index);

        // Kart 1
        card1Title.text = abilityNames[ability1Index];
        card1Desc.text = abilityDescs[ability1Index];
        if (card1Icon != null) card1Icon.text = abilityIcons[ability1Index];

        // Kart 2
        card2Title.text = abilityNames[ability2Index];
        card2Desc.text = abilityDescs[ability2Index];
        if (card2Icon != null) card2Icon.text = abilityIcons[ability2Index];

        // Kart 3
        card3Title.text = abilityNames[ability3Index];
        card3Desc.text = abilityDescs[ability3Index];
        if (card3Icon != null) card3Icon.text = abilityIcons[ability3Index];
    }

    public void SelectAbility1()
    {
        ApplyAbility(ability1Index);
        HideLevelUpPanel();
    }

    public void SelectAbility2()
    {
        ApplyAbility(ability2Index);
        HideLevelUpPanel();
    }

    public void SelectAbility3()
    {
        ApplyAbility(ability3Index);
        HideLevelUpPanel();
    }

    void ApplyAbility(int index)
    {
        switch (index)
        {
            case 0: // Saldırı Hasarı
                playerController.IncreaseAttackDamage(5);
                break;
            case 1: // Saldırı Hızı
                playerController.IncreaseAttackSpeed(0.1f);
                break;
            case 2: // Saldırı Menzili
                playerController.IncreaseAttackRange(0.5f);
                break;
            case 3: // Can Yenileme
                playerStats.HealHP(10);
                break;
            case 4: // Çift Hasar
                playerController.IncreaseAttackDamage(playerController.attackDamage);
                break;
            case 5: // Zırh
                playerStats.IncreaseArmor(2);
                break;
            case 6: // Ateş Hasarı
                playerController.IncreaseFireDamage(3);
                break;
            case 7: // Max Can Artışı
                playerStats.IncreaseMaxHP(20);
                break;
            case 8: // Güçlü Darbe
                playerController.IncreaseAttackDamage(10);
                break;
            case 9: // Süper Hız
                playerController.ActivateSuperSpeed();
                break;
        }

        Debug.Log($"Yetenek seçildi: {abilityNames[index]}");
    }

    void HideLevelUpPanel()
    {
        levelUpPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}