using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActiveAbilityUIManager : MonoBehaviour
{
    [System.Serializable]
    public class AbilityUISlot
    {
        public Button slotButton;         // YENİ EKLENDİ: Tıklama özelliği
        public Image iconImage;           
        public Image cooldownOverlay;     
        public TextMeshProUGUI cooldownText; 
        public TextMeshProUGUI nameText;  
    }

    [Header("UI Slotları (0, 1, 2)")]
    public AbilityUISlot[] uiSlots = new AbilityUISlot[3];

    [Header("Görseller (İsteğe Bağlı)")]
    public Sprite celestialStrikeSprite;
    public Sprite absoluteShieldSprite;
    public Sprite battleCrySprite;
    public Sprite emptySlotSprite; 

    private void OnEnable()
    {
        ActiveAbilityManager.onCooldownChanged += UpdateCooldownUI;
    }

    private void OnDisable()
    {
        ActiveAbilityManager.onCooldownChanged -= UpdateCooldownUI;
    }

    private void Start()
    {
        SetupInitialUI();

        // Tıklama komutlarını Unity üzerinden değil, direkt koddan bağlıyoruz (Çok daha güvenli!)
        if (uiSlots.Length > 0 && uiSlots[0].slotButton != null)
            uiSlots[0].slotButton.onClick.AddListener(() => OnAbilityClicked(0));
        
        if (uiSlots.Length > 1 && uiSlots[1].slotButton != null)
            uiSlots[1].slotButton.onClick.AddListener(() => OnAbilityClicked(1));
        
        if (uiSlots.Length > 2 && uiSlots[2].slotButton != null)
            uiSlots[2].slotButton.onClick.AddListener(() => OnAbilityClicked(2));
    }

    private void Update()
    {
        // OYUNCU DOSTU ÖZELLİK: Mouse yerine klavyedeki 1, 2 ve 3 tuşlarına basarak da büyü atılabilir!
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) OnAbilityClicked(0);
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) OnAbilityClicked(1);
        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) OnAbilityClicked(2);
    }

    // Hem butona basınca hem de klavyeden tuşa basınca çalışan ortak fonksiyon
    public void OnAbilityClicked(int slotIndex)
    {
        if (ActiveAbilityManager.Instance != null)
        {
            ActiveAbilityManager.Instance.CastAbility(slotIndex);
        }
    }

    void SetupInitialUI()
    {
        SetSlotVisual(0, celestialStrikeSprite, "Göksel Çarpma");
        SetSlotVisual(1, absoluteShieldSprite, "Mutlak Kalkan");
        SetSlotVisual(2, battleCrySprite, "Savaş Çığlığı");

        for (int i = 0; i < 3; i++)
        {
            if (uiSlots[i].cooldownOverlay != null) uiSlots[i].cooldownOverlay.fillAmount = 0f;
            if (uiSlots[i].cooldownText != null) uiSlots[i].cooldownText.text = "";
        }
    }

    void SetSlotVisual(int slotIndex, Sprite sprite, string abilityName)
    {
        if (slotIndex < 0 || slotIndex >= uiSlots.Length) return;

        if (uiSlots[slotIndex].iconImage != null)
        {
            uiSlots[slotIndex].iconImage.sprite = sprite != null ? sprite : emptySlotSprite;
            uiSlots[slotIndex].iconImage.color = sprite != null ? Color.white : new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }

        if (uiSlots[slotIndex].nameText != null)
        {
            uiSlots[slotIndex].nameText.text = abilityName;
        }
    }

    private void UpdateCooldownUI(int slot, float remaining, float total)
    {
        if (slot < 0 || slot >= uiSlots.Length) return;

        AbilityUISlot currentSlot = uiSlots[slot];

        if (remaining > 0)
        {
            if (currentSlot.cooldownOverlay != null) currentSlot.cooldownOverlay.fillAmount = remaining / total;
            if (currentSlot.cooldownText != null) currentSlot.cooldownText.text = Mathf.CeilToInt(remaining).ToString();
        }
        else
        {
            if (currentSlot.cooldownOverlay != null) currentSlot.cooldownOverlay.fillAmount = 0f;
            if (currentSlot.cooldownText != null) currentSlot.cooldownText.text = "";
        }
    }
}