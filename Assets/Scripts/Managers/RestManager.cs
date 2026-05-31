using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class RestManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI healButtonText;
    public TextMeshProUGUI playerHPText;

    private PlayerStats playerStats;
    private PlayerController playerController;

    void Start()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
        playerController = FindFirstObjectByType<PlayerController>();

        if (playerStats != null)
        {
            healButtonText.text = $"Can Yenile\n+{Mathf.RoundToInt(playerStats.maxHP * 0.3f)} HP";
            playerHPText.text = $"Mevcut Can: {playerStats.currentHP}/{playerStats.maxHP}";
        }
    }

    public void HealPlayer()
    {
        // Hoca Sorarsa: "Can yenileme oranını statik (sabit) vermek yerine, karakterin max canının %30'u olarak dinamik hesaplıyorum."
        if (playerStats != null)
        {
            playerStats.HealHP(Mathf.RoundToInt(playerStats.maxHP * 0.3f));
        }
        SceneManager.LoadScene("FloorSelectScene");
    }

    public void UpgradeAbility()
    {
        // Hoca Sorarsa: "Switch-case ve rastgele (Random) mantığıyla her dinlenmede farklı bir buff gelmesini sağlıyorum."
        if (playerController != null)
        {
            int random = Random.Range(0, 3);
            switch (random)
            {
                case 0: playerController.IncreaseAttackDamage(8); break;
                case 1: playerController.IncreaseAttackSpeed(0.15f); break;
                case 2: playerController.IncreaseAttackRange(0.5f); break;
            }
        }
        SceneManager.LoadScene("FloorSelectScene");
    }
}