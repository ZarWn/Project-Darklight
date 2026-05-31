using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public TextMeshProUGUI playerGoldText;

    // Hoca Sorarsa: "UI elemanlarını tek tek tanımlamak (Spaghetti Code) yerine Diziler (Array) kullanarak kod tekrarını (DRY prensibi) engelledim."
    [Header("Arayüz Dizileri (Inspector'dan Ekle)")]
    public TextMeshProUGUI[] nameTexts = new TextMeshProUGUI[4];
    public TextMeshProUGUI[] descTexts = new TextMeshProUGUI[4];
    public TextMeshProUGUI[] priceTexts = new TextMeshProUGUI[4];
    public Button[] buyButtons = new Button[4];

    private PlayerStats stats;
    private int[] selectedItems = new int[4];
    
    private string[] itemNames = { "Can Yenileme", "Güç Taşı", "Hız Ruhu", "Zırh Parçası", "Alev Tozu", "Kalkan Ruhu", "Menzil Taşı", "Ölüm Dokunuşu" };
    private string[] itemDescs = { "30 Can Yeniler", "+10 Hasar", "Saldırı Hızı", "+3 Zırh", "+5 Ateş Hasarı", "5sn Ölümsüzlük", "+1 Menzil", "Kritik Şansı" };
    private int[] itemPrices = { 30, 50, 40, 35, 45, 80, 40, 60 };

    void Start()
    {
        stats = FindFirstObjectByType<PlayerStats>();
        SelectRandomItems();
        UpdateGoldText();
    }

    void SelectRandomItems()
    {
        // Hoca Sorarsa: "Aynı eşyanın markette 2 kez çıkmaması için basit bir karıştırma (List & RemoveAt) mantığı kurdum."
        System.Collections.Generic.List<int> available = new System.Collections.Generic.List<int> { 0, 1, 2, 3, 4, 5, 6, 7 };
        
        for (int i = 0; i < 4; i++)
        {
            int randomIndex = Random.Range(0, available.Count);
            selectedItems[i] = available[randomIndex];
            available.RemoveAt(randomIndex);

            nameTexts[i].text = itemNames[selectedItems[i]];
            descTexts[i].text = itemDescs[selectedItems[i]];
            priceTexts[i].text = $"{itemPrices[selectedItems[i]]} Altın";
        }
    }

    void UpdateGoldText() { if (stats != null) playerGoldText.text = $"Altın: {stats.gold}"; }

    // NOT: Butonların Unity Inspector'daki OnClick kısmında bu fonksiyona 0, 1, 2, 3 parametrelerini vermelisin!
    public void BuyItem(int slot) 
    {
        if (stats == null) return;
        int itemIndex = selectedItems[slot];

        // Hoca Sorarsa: "Eğer oyuncu eşyayı alabiliyorsa, tekrar alamasın diye butonu anında deaktif (interactable=false) yapıyorum."
        if (stats.SpendGold(itemPrices[itemIndex]))
        {
            ApplyItem(itemIndex);
            UpdateGoldText();
            buyButtons[slot].interactable = false;
        }
    }

    void ApplyItem(int itemIndex)
    {
        PlayerController player = FindFirstObjectByType<PlayerController>();
        switch (itemIndex)
        {
            case 0: stats.HealHP(30); break;
            case 1: player?.IncreaseAttackDamage(10); break;
            case 2: player?.IncreaseAttackSpeed(0.1f); break;
            case 3: stats.IncreaseArmor(3); break;
            case 4: player?.IncreaseFireDamage(5); break;
            case 5: stats.isInvincible = true; break;
            case 6: player?.IncreaseAttackRange(1f); break;
            case 7: player?.IncreaseAttackDamage(15); break; 
        }
    }

    public void Continue() => SceneManager.LoadScene("FloorSelectScene");
}