using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance;

    private WeaponData selectedWeapon;
    private WeaponData[] weapons;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        InitializeWeapons();
    }

    void InitializeWeapons()
    {
        weapons = new WeaponData[5];

        // 1. Gece Bıçağı
        weapons[0] = new WeaponData
        {
            weaponName = "Gece Bıçağı",
            weaponDescription = "Karanlık zindanlardan çıkmış keskin bir bıçak.",
            damage = 20,
            attackSpeed = 0.3f,
            range = 1.5f,
            pros = new string[] {
                "Her 3 vuruşta kritik hasar (x2)",
                "Yüksek saldırı hızı"
            },
            cons = new string[] {
                "Menzil çok kısa",
                "Savunma -10"
            },
            weaponType = WeaponType.GeceBicagi,
            armorPenalty = 10,
            critMultiplier = 2f,
            critEvery = 3
        };

        // 2. Rün Yayı
        weapons[1] = new WeaponData
        {
            weaponName = "Rün Yayı",
            weaponDescription = "Antik rünlerle güçlendirilmiş büyülü bir yay.",
            damage = 15,
            attackSpeed = 0.8f,
            range = 4f,
            pros = new string[] {
                "Uzun menzil",
                "Oklar düşmandan geçer"
            },
            cons = new string[] {
                "Düşük hasar",
                "Yavaş saldırı hızı"
            },
            weaponType = WeaponType.RunYayi,
            piercingShot = true
        };

        // 3. Khaos Asası
        weapons[2] = new WeaponData
        {
            weaponName = "Khaos Asası",
            weaponDescription = "Kaotik bir güçle dolu, tüm düşmanlara hasar veren asa.",
            damage = 10,
            attackSpeed = 1.2f,
            range = 2.5f,
            pros = new string[] {
                "Tüm menzildeki düşmanlara vurur",
                "Geniş menzil"
            },
            cons = new string[] {
                "En düşük tek hasar",
                "Her saldırıda 2 can kaybı"
            },
            weaponType = WeaponType.KhaosAsasi,
            aoeAttack = true,
            selfDamage = 2
        };

        // 4. Kan Mızrağı
        weapons[3] = new WeaponData
        {
            weaponName = "Kan Mızrağı",
            weaponDescription = "Düşmanı kanatan, sürekli hasar veren bir mızrak.",
            damage = 18,
            attackSpeed = 0.6f,
            range = 2.5f,
            pros = new string[] {
                "Kanama uygular (3sn x 3 hasar)",
                "Orta menzil"
            },
            cons = new string[] {
                "Her kanamada 1 hasar alırsın",
                "Orta hasar"
            },
            weaponType = WeaponType.KanMizragi,
            applyBleed = true,
            bleedDamage = 3,
            bleedDuration = 3f,
            bleedSelfDamage = 1
        };

        // 5. Ruh Tırpanı
        weapons[4] = new WeaponData
        {
            weaponName = "Ruh Tırpanı",
            weaponDescription = "Düşmanın canını çalan gizemli bir tırpan.",
            damage = 12,
            attackSpeed = 0.7f,
            range = 2f,
            pros = new string[] {
                "Verilen hasarın %30u can olarak döner",
                "Orta menzil"
            },
            cons = new string[] {
                "Düşük hasar",
                "Maksimum can -20"
            },
            weaponType = WeaponType.RuhTirpani,
            lifeSteal = 0.3f,
            maxHPPenalty = 20
        };
    }

    public WeaponData GetWeapon(int index)
    {
        if (index >= 0 && index < weapons.Length)
            return weapons[index];
        return null;
    }

    public WeaponData GetSelectedWeapon()
    {
        return selectedWeapon;
    }

    public void SelectWeapon(int index)
    {
        selectedWeapon = weapons[index];
        Debug.Log($"Silah seçildi: {selectedWeapon.weaponName}");
    }

    public int GetWeaponCount()
    {
        return weapons.Length;
    }
}