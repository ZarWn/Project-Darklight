using UnityEngine;
using TMPro;

public class LevelUpManager : MonoBehaviour
{
    [Header("UI Referansları")]
    public GameObject levelUpPanel;
    public TextMeshProUGUI[] cardTitles = new TextMeshProUGUI[3];
    public TextMeshProUGUI[] cardDescs = new TextMeshProUGUI[3];
    
    // 12 Farklı Güçlü Pasif Yetenek
    private string[] names = { 
        "Kudretli Vuruş", "Rüzgarın Hızı", "Kartal Gözü", "Şifacının Dokunuşu", 
        "Demir İrade", "Alevli Kılıç", "Devin Kalbi", "Midas'ın Eli", 
        "Bilgenin Kitabı", "Vampir Dişi", "Gölge Adımı", "Berserker Yemini" 
    };
    
    private string[] descs = { 
        "+5 Hasar verir.", 
        "Saldırı hızını artırır.", 
        "+0.5 Saldırı menzili.", 
        "Maksimum canın %40'ını yeniler.", 
        "Alınan hasarı 2 azaltır (+2 Zırh).", 
        "Tüm saldırılara +5 Ateş hasarı.", 
        "+20 Maksimum Can sınırı.", 
        "Düşmanlardan %25 daha fazla altın düşer.", 
        "Düşmanlardan %25 daha fazla XP gelir.", 
        "Düşmana her vuruşta %15 ihtimalle 2 Can çalar.", 
        "%10 ihtimalle gelen hasardan tamamen kaçınır.", 
        "Maksimum canı 15 düşürür ama Hasarı 15 artırır." 
    };
    
    private int[] selectedIndexes = new int[3];

    public void ShowLevelUpPanel()
    {
        Time.timeScale = 0f; 
        SelectRandomAbilities();
        levelUpPanel.SetActive(true);
    }

    void SelectRandomAbilities()
    {
        selectedIndexes[0] = Random.Range(0, names.Length);
        do { selectedIndexes[1] = Random.Range(0, names.Length); } while (selectedIndexes[1] == selectedIndexes[0]);
        do { selectedIndexes[2] = Random.Range(0, names.Length); } while (selectedIndexes[2] == selectedIndexes[0] || selectedIndexes[2] == selectedIndexes[1]);

        for (int i = 0; i < 3; i++)
        {
            if (cardTitles[i]) cardTitles[i].text = names[selectedIndexes[i]];
            if (cardDescs[i]) cardDescs[i].text = descs[selectedIndexes[i]];
        }
    }

    public void SelectAbility(int cardIndex) 
    {
        ApplyAbility(selectedIndexes[cardIndex]);
        levelUpPanel.SetActive(false);
        Time.timeScale = 1f; 
    }

    void ApplyAbility(int index)
    {
        PlayerController player = PlayerController.Instance;
        PlayerStats stats = PlayerStats.Instance;

        if (player == null || stats == null) return;

        switch (index)
        {
            case 0: player.IncreaseAttackDamage(5); break; // Kudretli Vuruş
            case 1: player.IncreaseAttackSpeed(0.1f); break; // Rüzgarın Hızı
            case 2: player.IncreaseAttackRange(0.5f); break; // Kartal Gözü
            case 3: stats.HealHP(Mathf.RoundToInt(stats.maxHP * 0.4f)); break; // Şifacının Dokunuşu
            case 4: stats.IncreaseArmor(2); break; // Demir İrade
            case 5: player.IncreaseFireDamage(5); break; // Alevli Kılıç
            case 6: stats.IncreaseMaxHP(20); break; // Devin Kalbi
            case 7: stats.goldMultiplier += 0.25f; break; // Midas'ın Eli
            case 8: stats.xpMultiplier += 0.25f; break; // Bilgenin Kitabı
            case 9: player.lifestealChance += 0.15f; break; // Vampir Dişi
            case 10: stats.dodgeChance += 0.10f; break; // Gölge Adımı
            case 11: // Berserker Yemini
                stats.IncreaseMaxHP(-15); 
                player.IncreaseAttackDamage(15); 
                break; 
        }
    }
}