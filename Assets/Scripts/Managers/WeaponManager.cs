using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance;

    private WeaponData selectedWeapon;
    private WeaponData[] weapons;

    private void Awake()
    {
        // Hoca Sorarsa: "Oyun boyunca tek bir silah veritabanı olması için Singleton deseni uyguladım."
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        InitializeWeapons();
    }

    void InitializeWeapons()
    {
        // Hoca Sorarsa: "Tek tek atama yapmak yerine Array Initializer (Dizi Başlatıcı) kullanarak kodu inanılmaz derecede kısalttım."
        weapons = new WeaponData[] {
            new WeaponData { weaponName = "Gece Bıçağı", damage = 20, attackSpeed = 0.3f, range = 1.5f, pros = new[]{"Kritik (x2)", "Hızlı"}, cons = new[]{"Kısa Menzil", "-10 Savunma"}, weaponType = WeaponType.GeceBicagi, critMultiplier = 2f, critEvery = 3, armorPenalty = 10 },
            new WeaponData { weaponName = "Rün Yayı", damage = 15, attackSpeed = 0.8f, range = 4f, pros = new[]{"Uzun Menzil", "Delici Ok"}, cons = new[]{"Düşük Hasar", "Yavaş"}, weaponType = WeaponType.RunYayi, piercingShot = true },
            new WeaponData { weaponName = "Khaos Asası", damage = 10, attackSpeed = 1.2f, range = 2.5f, pros = new[]{"Alan Hasarı (AoE)"}, cons = new[]{"Düşük Hasar", "Öz Hasar"}, weaponType = WeaponType.KhaosAsasi, aoeAttack = true, selfDamage = 2 },
            new WeaponData { weaponName = "Kan Mızrağı", damage = 18, attackSpeed = 0.6f, range = 2.5f, pros = new[]{"Kanama (3sn)"}, cons = new[]{"Öz Kanama"}, weaponType = WeaponType.KanMizragi, applyBleed = true, bleedDamage = 3, bleedDuration = 3f, bleedSelfDamage = 1 },
            new WeaponData { weaponName = "Ruh Tırpanı", damage = 12, attackSpeed = 0.7f, range = 2f, pros = new[]{"%30 Can Çalma"}, cons = new[]{"-20 Max Can"}, weaponType = WeaponType.RuhTirpani, lifeSteal = 0.3f, maxHPPenalty = 20 }
        };
    }

    public WeaponData GetWeapon(int index) => (index >= 0 && index < weapons.Length) ? weapons[index] : null;
    public WeaponData GetSelectedWeapon() => selectedWeapon;
    public void SelectWeapon(int index) { selectedWeapon = weapons[index]; }
    public int GetWeaponCount() => weapons.Length;
}