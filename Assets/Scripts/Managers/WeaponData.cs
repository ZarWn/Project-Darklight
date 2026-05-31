using UnityEngine;

// Hoca Sorarsa: "Silah verilerini kalıp olarak tutmak için Serializable (Serileştirilebilir) bir model sınıfı oluşturdum. Bu, bellek yönetimini kolaylaştırır."
[System.Serializable]
public class WeaponData
{
    public string weaponName, weaponDescription;
    public int damage;
    public float attackSpeed, range;
    public string[] pros, cons;
    public WeaponType weaponType;

    public int armorPenalty, critEvery, selfDamage, bleedDamage, bleedSelfDamage, maxHPPenalty;
    public float critMultiplier, bleedDuration, lifeSteal;
    public bool piercingShot, aoeAttack, applyBleed;
}

// Yeni efsanevi kılıç listemiz
public enum WeaponType { PasliCirakKilici, SuikastciKisaKilici, SovalyeUzunKilici, LanetliKatana, CellatBuyukKilici }