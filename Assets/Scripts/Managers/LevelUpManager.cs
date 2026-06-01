using UnityEngine;
using TMPro;

public class LevelUpManager : MonoBehaviour
{
    [Header("UI Referansları")]
    public GameObject levelUpPanel;
    public TextMeshProUGUI[] cardTitles = new TextMeshProUGUI[3];
    public TextMeshProUGUI[] cardDescs = new TextMeshProUGUI[3];
    
    // --- 17 FARKLI GÜÇLÜ VE YÜZDELİK DENGELİ YETENEK ---
    private string[] names = { 
        "Kudretli Vuruş", "Rüzgarın Hızı", "Kartal Gözü", "Şifacının Dokunuşu", 
        "Demir İrade", "Alevli Kılıç", "Devin Kalbi", "Midas'ın Eli", 
        "Bilgenin Kitabı", "Vampir Dişi", "Gölge Adımı", "Berserker Yemini",
        "Gölge Dansçısı", "Sarsılmaz", "Açgözlü Tüccar", "Cehennem Ateşi", "Yaşam Ağacı" 
    };
    
    private string[] descs = { 
        "+5 Hasar verir.", 
        "Saldırı hızını artırır.", 
        "+0.5 Saldırı menzili.", 
        "Maksimum canın %40'ını yeniler.", 
        "Alınan hasarı 2 azaltır (+2 Zırh).", 
        "Tüm saldırılara +5 Ateş hasarı.", 
        "Maksimum Can sınırını %20 artırır.", // GÜNCELLENDİ (Yüzdelik)
        "Düşmanlardan %25 daha fazla altın düşer.", 
        "Düşmanlardan %25 daha fazla XP gelir.", 
        "Düşmana her vuruşta %15 ihtimalle 2 Can çalar.", 
        "%10 ihtimalle gelen hasardan tamamen kaçınır.", 
        "15 Can feda ederek Hasarı 15 artırır. (Kalkanı deler!)",
        "Saldırı hızını hafif artırır ve +%4 Kaçınma sağlar.",
        "+1 Zırh verir ve Maksimum Canı %10 artırır.", // GÜNCELLENDİ (Yüzdelik)
        "Altın kazancını %40 artırır ama Maksimum Canı %15 düşürür.", // GÜNCELLENDİ (Yüzdelik)
        "Saldırılara +4 Ateş Hasarı ve +2 Normal Hasar ekler.",
        "Maksimum canın %30'unu yeniler ve +%8 Can Çalma ihtimali verir."
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
            case 0: player.IncreaseAttackDamage(5); break; 
            case 1: player.IncreaseAttackSpeed(0.1f); break; 
            case 2: player.IncreaseAttackRange(0.5f); break; 
            case 3: stats.HealHP(Mathf.RoundToInt(stats.maxHP * 0.4f)); break; 
            case 4: stats.IncreaseArmor(2); break; 
            case 5: player.IncreaseFireDamage(5); break; 
            
            // --- HATA ÇÖZÜMÜ: YÜZDELİK ARTIRIM MANTIĞI ---
            case 6: // Devin Kalbi
                int devinKalbiBonus = Mathf.RoundToInt(stats.maxHP * 0.20f);
                stats.IncreaseMaxHP(devinKalbiBonus); 
                break; 
                
            case 7: stats.goldMultiplier += 0.25f; break; 
            case 8: stats.xpMultiplier += 0.25f; break; 
            case 9: player.lifestealChance += 0.15f; break; 
            case 10: stats.dodgeChance += 0.10f; break; 
            
            case 11: // Berserker Yemini
                if (stats.currentHP > 15) stats.currentHP -= 15; 
                else stats.currentHP = 1; 
                player.IncreaseAttackDamage(15); 
                break; 
                
            case 12: // Gölge Dansçısı 
                player.IncreaseAttackSpeed(0.06f);
                stats.dodgeChance += 0.04f;
                break;
                
            case 13: // Sarsılmaz 
                stats.IncreaseArmor(1);
                int sarsilmazBonus = Mathf.RoundToInt(stats.maxHP * 0.10f);
                stats.IncreaseMaxHP(sarsilmazBonus);
                break;
                
            case 14: // Açgözlü Tüccar
                stats.goldMultiplier += 0.40f;
                int silinecekCan = Mathf.RoundToInt(stats.maxHP * 0.15f);
                if (stats.maxHP > silinecekCan + 1) stats.IncreaseMaxHP(-silinecekCan); 
                else stats.maxHP = 1; 
                break;
                
            case 15: // Cehennem Ateşi 
                player.IncreaseFireDamage(4);
                player.IncreaseAttackDamage(2);
                break;
                
            case 16: // Yaşam Ağacı 
                stats.HealHP(Mathf.RoundToInt(stats.maxHP * 0.3f));
                player.lifestealChance += 0.08f;
                break;
        }
    }
}