using UnityEngine;
using TMPro;

public class LevelUpManager : MonoBehaviour
{
    [Header("UI Referansları")]
    public GameObject levelUpPanel;
    public TextMeshProUGUI[] cardTitles = new TextMeshProUGUI[3];
    public TextMeshProUGUI[] cardDescs = new TextMeshProUGUI[3];
    
    public PlayerController player;
    public PlayerStats stats;

    // Hoca Sorarsa: "Yetenekleri iki ayrı Array (Dizi) içinde tutarak kod kalabalığını önledim."
    private string[] names = { "Saldırı", "Hız", "Menzil", "Can", "Çift Hasar", "Zırh", "Ateş", "Max Can", "Güçlü Darbe", "Süper Hız" };
    private string[] descs = { "+5 Hasar", "Hız Artar", "+0.5 Menzil", "10 Can Yeniler", "Hasar x2", "2 Zırh", "Ateş Hasarı", "+20 Max Can", "+10 Hasar", "Max Hız" };
    private int[] selectedIndexes = new int[3];

    public void ShowLevelUpPanel()
    {
        Time.timeScale = 0f; // Hoca Sorarsa: "Panel açılınca arkada oyun akmasın diye zamanı durduruyoruz."
        SelectRandomAbilities();
        levelUpPanel.SetActive(true);
    }

    void SelectRandomAbilities()
    {
        // Hoca Sorarsa: "Aynı yetenek 2 kere gelmesin diye basit bir kontrol döngüsü (do-while) kurdum."
        selectedIndexes[0] = Random.Range(0, names.Length);
        do { selectedIndexes[1] = Random.Range(0, names.Length); } while (selectedIndexes[1] == selectedIndexes[0]);
        do { selectedIndexes[2] = Random.Range(0, names.Length); } while (selectedIndexes[2] == selectedIndexes[0] || selectedIndexes[2] == selectedIndexes[1]);

        for (int i = 0; i < 3; i++)
        {
            cardTitles[i].text = names[selectedIndexes[i]];
            cardDescs[i].text = descs[selectedIndexes[i]];
        }
    }

    // Butonlara UI'dan verilecek fonksiyonlar
    public void SelectAbility(int cardIndex) // 0, 1 veya 2 gönderilir
    {
        ApplyAbility(selectedIndexes[cardIndex]);
        levelUpPanel.SetActive(false);
        Time.timeScale = 1f; // Zamanı geri akıt
    }

    void ApplyAbility(int index)
    {
        // Hoca Sorarsa: "Switch-case yapısı if-else karmaşasına göre çok daha performanslı ve okunabilirdir."
        switch (index)
        {
            case 0: player.IncreaseAttackDamage(5); break;
            case 1: player.IncreaseAttackSpeed(0.1f); break;
            case 2: player.IncreaseAttackRange(0.5f); break;
            case 3: stats.HealHP(10); break;
            case 4: player.IncreaseAttackDamage(player.attackDamage); break;
            case 5: stats.IncreaseArmor(2); break;
            case 6: player.IncreaseFireDamage(3); break;
            case 7: stats.IncreaseMaxHP(20); break;
            case 8: player.IncreaseAttackDamage(10); break;
            case 9: player.ActivateSuperSpeed(); break;
        }
    }
}