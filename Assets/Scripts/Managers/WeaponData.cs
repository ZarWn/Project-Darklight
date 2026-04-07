using UnityEngine;

[System.Serializable]
public class WeaponData
{
    public string weaponName;
    public string weaponDescription;
    public int damage;
    public float attackSpeed;
    public float range;
    public string[] pros;
    public string[] cons;
    public WeaponType weaponType;

    // Silaha özel özellikler
    public int armorPenalty;          // Gece Bıçağı: savunma azalması
    public float critMultiplier;      // Gece Bıçağı: kritik çarpanı
    public int critEvery;             // Gece Bıçağı: kaç vuruşta bir kritik
    public bool piercingShot;         // Rün Yayı: düşmandan geçen ok
    public bool aoeAttack;            // Khaos Asası: alan saldırısı
    public int selfDamage;            // Khaos Asası: kendine hasar
    public bool applyBleed;           // Kan Mızrağı: kanama
    public int bleedDamage;           // Kan Mızrağı: kanama hasarı
    public float bleedDuration;       // Kan Mızrağı: kanama süresi
    public int bleedSelfDamage;       // Kan Mızrağı: kendine kanama hasarı
    public float lifeSteal;           // Ruh Tırpanı: can çalma yüzdesi
    public int maxHPPenalty;          // Ruh Tırpanı: max can azalması
}

public enum WeaponType
{
    GeceBicagi,
    RunYayi,
    KhaosAsasi,
    KanMizragi,
    RuhTirpani
}