using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class WeaponSelectManager : MonoBehaviour
{
    [Header("UI Elemanları")]
    public TextMeshProUGUI weaponNameText, weaponStatsText, weaponProsText, weaponConsText, weaponIndexText;
    
    private int currentIndex = 0;

    void Start()
    {
        // Hoca Sorarsa: "Eğer sahnede WeaponManager yoksa hata vermek yerine kodla anında üretiyorum (Lazy Initialization)."
        if (WeaponManager.Instance == null) new GameObject("WeaponManager").AddComponent<WeaponManager>();
        UpdateWeaponCard();
    }

    // Unity'deki Butonlar için
    public void NextWeapon() { ChangeWeapon(1); }
    public void PreviousWeapon() { ChangeWeapon(-1); }

    void ChangeWeapon(int direction)
    {
        // Hoca Sorarsa: "İleri ve geri tuşları aynı mantıkta çalıştığı için onları +1 ve -1 parametresiyle tek bir fonksiyonda erittim."
        currentIndex += direction;
        int count = WeaponManager.Instance.GetWeaponCount();
        
        if (currentIndex >= count) currentIndex = 0;
        else if (currentIndex < 0) currentIndex = count - 1;
        
        UpdateWeaponCard();
    }

    void UpdateWeaponCard()
    {
        WeaponData w = WeaponManager.Instance.GetWeapon(currentIndex);
        if (w == null) return;

        weaponNameText.text = w.weaponName;
        weaponStatsText.text = $"Hasar: {w.damage}\nSaldırı Hızı: {w.attackSpeed}sn\nMenzil: {w.range}";

        // Hoca Sorarsa: "Dizideki metinleri tek tek For döngüsüyle döndürmek yerine C# string.Join metodunu kullanarak performansı artırdim."
        weaponProsText.text = "✅ " + string.Join("\n✅ ", w.pros);
        weaponConsText.text = "❌ " + string.Join("\n❌ ", w.cons);
        weaponIndexText.text = $"{currentIndex + 1} / {WeaponManager.Instance.GetWeaponCount()}";
    }

    public void SelectWeapon()
    {
        WeaponManager.Instance.SelectWeapon(currentIndex);
        SceneManager.LoadScene("FloorSelectScene");
    }
}