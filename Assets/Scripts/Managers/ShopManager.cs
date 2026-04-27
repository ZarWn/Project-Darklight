using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("UI Elemanları")]
    public TextMeshProUGUI playerGoldText;

    [Header("Kart İsimleri")]
    public TextMeshProUGUI item1Name;
    public TextMeshProUGUI item2Name;
    public TextMeshProUGUI item3Name;
    public TextMeshProUGUI item4Name;

    [Header("Kart Açıklamaları")]
    public TextMeshProUGUI item1Desc;
    public TextMeshProUGUI item2Desc;
    public TextMeshProUGUI item3Desc;
    public TextMeshProUGUI item4Desc;

    [Header("Kart Fiyatları")]
    public TextMeshProUGUI item1Price;
    public TextMeshProUGUI item2Price;
    public TextMeshProUGUI item3Price;
    public TextMeshProUGUI item4Price;

    [Header("Satın Al Butonları")]
    public Button buyButton1;
    public Button buyButton2;
    public Button buyButton3;
    public Button buyButton4;

    private PlayerStats playerStats;

    // Eşya verileri
    private string[] itemNames = {
        "Can Yenileme",
        "Güç Taşı",
        "Hız Ruhu",
        "Zırh Parçası",
        "Alev Tozu",
        "Kalkan Ruhu",
        "Menzil Taşı",
        "Ölüm Dokunuşu"
    };

    private string[] itemDescs = {
        "Aninda 30 can yenile",
        "Saldiri hasari +10 artar",
        "Saldiri hizi artar",
        "Zirh +3 artar",
        "Ates hasari +5 artar",
        "5 saniye hasar almaz",
        "Saldiri menzili +1 artar",
        "Kritik hasar sansi artar"
    };

    private int[] itemPrices = { 30, 50, 40, 35, 45, 80, 40, 60 };

    private int[] selectedItems = new int[4];

    void Start()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
        SelectRandomItems();
        UpdateGoldText();
    }

    void SelectRandomItems()
    {
        System.Collections.Generic.List<int> available = new System.Collections.Generic.List<int>();
        for (int i = 0; i < itemNames.Length; i++)
            available.Add(i);

        for (int i = 0; i < 4; i++)
        {
            int randomIndex = Random.Range(0, available.Count);
            selectedItems[i] = available[randomIndex];
            available.RemoveAt(randomIndex);
        }

        UpdateItemCards();
    }

    void UpdateItemCards()
    {
        item1Name.text = itemNames[selectedItems[0]];
        item2Name.text = itemNames[selectedItems[1]];
        item3Name.text = itemNames[selectedItems[2]];
        item4Name.text = itemNames[selectedItems[3]];

        item1Desc.text = itemDescs[selectedItems[0]];
        item2Desc.text = itemDescs[selectedItems[1]];
        item3Desc.text = itemDescs[selectedItems[2]];
        item4Desc.text = itemDescs[selectedItems[3]];

        item1Price.text = $"{itemPrices[selectedItems[0]]} Altin";
        item2Price.text = $"{itemPrices[selectedItems[1]]} Altin";
        item3Price.text = $"{itemPrices[selectedItems[2]]} Altin";
        item4Price.text = $"{itemPrices[selectedItems[3]]} Altin";
    }

    void UpdateGoldText()
    {
        if (playerStats != null)
            playerGoldText.text = $"Altin: {playerStats.gold}";
    }

    public void BuyItem1() { BuyItem(0); }
    public void BuyItem2() { BuyItem(1); }
    public void BuyItem3() { BuyItem(2); }
    public void BuyItem4() { BuyItem(3); }

    void BuyItem(int slot)
    {
        if (playerStats == null) return;

        int itemIndex = selectedItems[slot];
        int price = itemPrices[itemIndex];

        if (playerStats.SpendGold(price))
        {
            ApplyItem(itemIndex);
            UpdateGoldText();

            // Satın alınan butonu kapat
            switch (slot)
            {
                case 0: buyButton1.interactable = false; break;
                case 1: buyButton2.interactable = false; break;
                case 2: buyButton3.interactable = false; break;
                case 3: buyButton4.interactable = false; break;
            }

            Debug.Log($"Satin alindi: {itemNames[itemIndex]}");
        }
        else
        {
            Debug.Log("Yeterli altin yok!");
        }
    }

    void ApplyItem(int itemIndex)
    {
        PlayerController playerController = FindFirstObjectByType<PlayerController>();

        switch (itemIndex)
        {
            case 0: // Can Yenileme
                playerStats.HealHP(30);
                break;
            case 1: // Güç Taşı
                if (playerController != null)
                    playerController.IncreaseAttackDamage(10);
                break;
            case 2: // Hız Ruhu
                if (playerController != null)
                    playerController.IncreaseAttackSpeed(0.1f);
                break;
            case 3: // Zırh Parçası
                playerStats.IncreaseArmor(3);
                break;
            case 4: // Alev Tozu
                if (playerController != null)
                    playerController.IncreaseFireDamage(5);
                break;
            case 5: // Kalkan Ruhu
                playerStats.isInvincible = true;
                break;
            case 6: // Menzil Taşı
                if (playerController != null)
                    playerController.IncreaseAttackRange(1f);
                break;
            case 7: // Ölüm Dokunuşu
                if (playerController != null)
                    playerController.IncreaseAttackDamage(15);
                break;
        }
    }

    public void Continue()
    {
        SceneManager.LoadScene("FloorSelectScene");
    }
}