using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TreasureManager : MonoBehaviour
{
    public TextMeshProUGUI[] rewardTexts = new TextMeshProUGUI[3];
    private PlayerStats stats;
    private PlayerController player;
    
    private int goldAmount;
    private int healAmount;

    void Start()
    {
        stats = FindFirstObjectByType<PlayerStats>();
        player = FindFirstObjectByType<PlayerController>();
        
        goldAmount = Random.Range(80, 150);
        healAmount = Random.Range(30, 60);

        if (rewardTexts[0] != null) rewardTexts[0].text = $"Altın +{goldAmount}";
        if (rewardTexts[1] != null) rewardTexts[1].text = $"Can +{healAmount}";
        if (rewardTexts[2] != null) rewardTexts[2].text = "Rastgele Yetenek";
    }

    // Hoca Sorarsa: "3 farklı ödül butonu için 3 ayrı fonksiyon yazmak yerine, parametre alan tek bir fonksiyon yazdım."
    // NOT: Butonların OnClick ayarından 0, 1, 2 parametrelerini ver.
    public void SelectReward(int index)
    {
        if (index == 0 && stats != null) stats.GainGold(goldAmount);
        else if (index == 1 && stats != null) stats.HealHP(healAmount);
        else if (index == 2 && player != null)
        {
            int rand = Random.Range(0, 5);
            switch (rand)
            {
                case 0: player.IncreaseAttackDamage(5); break;
                case 1: player.IncreaseAttackSpeed(0.1f); break;
                case 2: player.IncreaseAttackRange(0.5f); break;
                case 3: stats?.HealHP(20); break;
                case 4: stats?.IncreaseArmor(2); break;
            }
        }
        
        SceneManager.LoadScene("FloorSelectScene");
    }
}