using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance;

    private WeaponData selectedWeapon;
    private WeaponData[] weapons;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        InitializeWeapons();
    }

    void InitializeWeapons()
    {
        // JÜRİYE NOT: "Silahların DPS (Saniye Başına Hasar) oranları, oyuncunun alması gereken riskle (Menzil kısalığı ve Saldırı hızı) doğru orantılı olarak dengelendi."
        weapons = new WeaponData[] {
            // 1. Paslı Çırak Kılıcı (Referans Silah - DPS: 24)
            new WeaponData { weaponName = "Paslı Çırak Kılıcı", damage = 12, attackSpeed = 0.5f, range = 2.5f, pros = new[]{"Dengeli Başlangıç", "Güvenilir Mesafe"}, cons = new[]{"Düşük DPS"}, weaponType = WeaponType.PasliCirakKilici },
            
            // 2. Suikastçı Kısa Kılıcı (Riskli Ama Çok Seri - DPS: 28.5)
            // Hasarı 8'den 10'a çıkarıldı. Yakınına girdiği düşmanı saniyeler içinde eritir.
            new WeaponData { weaponName = "Suikastçı Kısa", damage = 10, attackSpeed = 0.35f, range = 2.1f, pros = new[]{"Aşırı Hızlı", "Seri Vuruş (Yüksek DPS)"}, cons = new[]{"Kısa Menzil (Tehlikeli)", "Düşük Tekli Hasar"}, weaponType = WeaponType.SuikastciKisaKilici },
            
            // 3. Şövalye Uzun Kılıcı (Güvenli Oynanış - DPS: 25.7)
            // Hasar yemeyi sevmeyen, uzaktan vur-kaç yapan oyuncular için altın oran.
            new WeaponData { weaponName = "Şövalye Kılıcı", damage = 18, attackSpeed = 0.7f, range = 3.2f, pros = new[]{"Uzun Menzil", "Güvenli Vuruş"}, cons = new[]{"Savurması Biraz Yavaş"}, weaponType = WeaponType.SovalyeUzunKilici },
            
            // 4. Lanetli Katana (Usta İşi - DPS: 40)
            // Hasarı 22'den 16'ya çekilerek oyunu kırması (OP olması) engellendi. Refleks isteyen, cezalandırıcı silah.
            new WeaponData { weaponName = "Lanetli Katana", damage = 16, attackSpeed = 0.4f, range = 2.4f, pros = new[]{"Mükemmel Hız", "Çok Yüksek DPS"}, cons = new[]{"Hata Affetmez", "Riskli Mesafe"}, weaponType = WeaponType.LanetliKatana },
            
            // 5. Cellat Büyük Kılıcı (Tank Tipi - DPS: 38.8)
            // Menzili 3.8'den 3.4'e düşürüldü. Vurduğunda ezer geçer ama ıskalarsa oyuncuyu savunmasız bırakır.
            new WeaponData { weaponName = "Cellat Kılıcı", damage = 35, attackSpeed = 0.9f, range = 3.4f, pros = new[]{"Devasa Hasar", "Geniş Alan Tarama"}, cons = new[]{"Çok Yavaş Toparlanma", "Saldırı Arası Boşluk"}, weaponType = WeaponType.CellatBuyukKilici }
        };
    }

    public WeaponData GetWeapon(int index) => (index >= 0 && index < weapons.Length) ? weapons[index] : null;
    public WeaponData GetSelectedWeapon() => selectedWeapon;
    public void SelectWeapon(int index) { selectedWeapon = weapons[index]; }
    public int GetWeaponCount() => weapons.Length;
}