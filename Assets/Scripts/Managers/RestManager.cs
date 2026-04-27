using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class RestManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI healButtonText;
    public TextMeshProUGUI upgradeButtonText;
    public TextMeshProUGUI playerHPText;

    private PlayerStats playerStats;
    private PlayerController playerController;

    void Start()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
        playerController = FindFirstObjectByType<PlayerController>();
        UpdateUI();
    }

    void UpdateUI()
    {
        if (playerStats == null) return;

        int healAmount = Mathf.RoundToInt(playerStats.maxHP * 0.3f);

        if (healButtonText != null)
            healButtonText.text = $"Can Yenile\n+{healAmount} HP";

        if (playerHPText != null)
            playerHPText.text = $"Mevcut Can: {playerStats.currentHP}/{playerStats.maxHP}";
    }

    public void HealPlayer()
    {
        if (playerStats == null) return;

        int healAmount = Mathf.RoundToInt(playerStats.maxHP * 0.3f);
        playerStats.HealHP(healAmount);
        Debug.Log($"Can yenilendi: +{healAmount}");

        SceneManager.LoadScene("FloorSelectScene");
    }

    public void UpgradeAbility()
    {
        if (playerController == null) return;

        // Rastgele bir yeteneği güçlendir
        int random = Random.Range(0, 3);
        switch (random)
        {
            case 0:
                playerController.IncreaseAttackDamage(8);
                Debug.Log("Saldiri hasari güçlendirildi!");
                break;
            case 1:
                playerController.IncreaseAttackSpeed(0.15f);
                Debug.Log("Saldiri hizi güçlendirildi!");
                break;
            case 2:
                playerController.IncreaseAttackRange(0.5f);
                Debug.Log("Saldiri menzili güçlendirildi!");
                break;
        }

        SceneManager.LoadScene("FloorSelectScene");
    }
}