using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TreasureManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI reward1Text;
    public TextMeshProUGUI reward2Text;
    public TextMeshProUGUI reward3Text;

    private PlayerStats playerStats;
    private PlayerController playerController;

    void Start()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
        playerController = FindFirstObjectByType<PlayerController>();
        GenerateRewards();
    }

    void GenerateRewards()
    {
        // Her seferinde farklı ödüller çıksın
        int random1 = Random.Range(80, 150);
        int random2 = Random.Range(30, 60);

        if (reward1Text != null) reward1Text.text = $"Altin +{random1}";
        if (reward2Text != null) reward2Text.text = $"Can +{random2}";
        if (reward3Text != null) reward3Text.text = "Rastgele Yetenek";
    }

    public void SelectReward1()
    {
        // Altın ödülü
        if (playerStats != null)
        {
            int amount = Random.Range(80, 150);
            playerStats.GainGold(amount);
            Debug.Log($"Altin kazanildi: +{amount}");
        }
        Continue();
    }

    public void SelectReward2()
    {
        // Can ödülü
        if (playerStats != null)
        {
            int amount = Random.Range(30, 60);
            playerStats.HealHP(amount);
            Debug.Log($"Can yenilendi: +{amount}");
        }
        Continue();
    }

    public void SelectReward3()
    {
        // Rastgele yetenek
        if (playerController != null)
        {
            int random = Random.Range(0, 5);
            switch (random)
            {
                case 0: playerController.IncreaseAttackDamage(5); break;
                case 1: playerController.IncreaseAttackSpeed(0.1f); break;
                case 2: playerController.IncreaseAttackRange(0.5f); break;
                case 3: playerStats.HealHP(20); break;
                case 4: playerStats.IncreaseArmor(2); break;
            }
            Debug.Log("Rastgele yetenek verildi!");
        }
        Continue();
    }

    void Continue()
    {
        SceneManager.LoadScene("FloorSelectScene");
    }
}