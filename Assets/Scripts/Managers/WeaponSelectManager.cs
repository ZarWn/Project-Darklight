using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class WeaponSelectManager : MonoBehaviour
{
    [Header("UI Elemanları")]
    public TextMeshProUGUI weaponNameText;
    public TextMeshProUGUI weaponStatsText;
    public TextMeshProUGUI weaponProsText;
    public TextMeshProUGUI weaponConsText;
    public TextMeshProUGUI weaponIndexText;

    private int currentWeaponIndex = 0;
    private WeaponManager weaponManager;

    void Start()
    {
        // WeaponManager yoksa oluştur
        if (WeaponManager.Instance == null)
        {
            GameObject wm = new GameObject("WeaponManager");
            wm.AddComponent<WeaponManager>();
        }

        weaponManager = WeaponManager.Instance;
        UpdateWeaponCard();
    }

    public void NextWeapon()
    {
        currentWeaponIndex++;
        if (currentWeaponIndex >= weaponManager.GetWeaponCount())
            currentWeaponIndex = 0;

        UpdateWeaponCard();
    }

    public void PreviousWeapon()
    {
        currentWeaponIndex--;
        if (currentWeaponIndex < 0)
            currentWeaponIndex = weaponManager.GetWeaponCount() - 1;

        UpdateWeaponCard();
    }

    void UpdateWeaponCard()
    {
        WeaponData weapon = weaponManager.GetWeapon(currentWeaponIndex);
        if (weapon == null) return;

        // Silah adı
        weaponNameText.text = weapon.weaponName;

        // Silah istatistikleri
        weaponStatsText.text =
            $"Hasar: {weapon.damage}\n" +
            $"Saldırı Hızı: {weapon.attackSpeed}sn\n" +
            $"Menzil: {weapon.range}";

        // Artılar
        string prosText = "";
        foreach (string pro in weapon.pros)
        {
            prosText += $"✅ {pro}\n";
        }
        weaponProsText.text = prosText;

        // Eksiler
        string consText = "";
        foreach (string con in weapon.cons)
        {
            consText += $"❌ {con}\n";
        }
        weaponConsText.text = consText;

        // Index göstergesi
        weaponIndexText.text = $"{currentWeaponIndex + 1} / {weaponManager.GetWeaponCount()}";
    }

    public void SelectWeapon()
    {
    weaponManager.SelectWeapon(currentWeaponIndex);
    SceneManager.LoadScene("FloorSelectScene");
    }
}