🎮 Wave Crusher

Wave Crusher, Unity 6 ile geliştirilmiş 2D mobil aksiyon türünde bir oyundur.

Oyuncu, ekranın ortasında sabit duran bir karakteri kontrol eder. Karakter otomatik olarak sağa ve sola saldırır. Düşmanlar dalgalar halinde gelir ve her dalgada güçlenir. Oyuncu XP kazanarak seviye atlar ve yeni yetenekler seçer. Son dalgada Boss’u yenerek bölümü tamamlar.

🚀 Oyun Özellikleri

⚔️ Otomatik saldırı sistemi

🌊 Dalga dalga gelen düşmanlar

📈 Her dalgada güçlenen düşmanlar

🧠 10 farklı yetenek seçim sistemi

👑 Boss dalgası

❤️ HP, XP ve seviye sistemi

🛡️ Zırh sistemi

🛠️ Kullanılan Teknolojiler

Unity 6 (6000.3.9f1)

C#

Unity Input System

Unity UI (TextMeshPro)

📥 Kurulum ve Çalıştırma
1. Repoyu klonlayın
git clone https://github.com/kullaniciadi/wavecrusher.git

2. Unity ile açın

Unity Hub'ı açın

Open butonuna tıklayın

Proje klasörünü seçin

Unity sürümü olarak 6000.3.9f1 kullanın

3. Oyunu çalıştırın

Assets/Scenes/GameScene.unity sahnesini açın

Unity editöründe Play butonuna basın

📁 Proje Klasör Yapısı
Assets/
├── Prefabs/
│   ├── Enemy.prefab
│   └── Boss.prefab
├── Scenes/
│   └── GameScene.unity
├── Sprites/
└── Scripts/
    ├── Player/
    │   ├── PlayerController.cs
    │   └── PlayerStats.cs
    ├── Enemy/
    │   ├── EnemyController.cs
    │   └── EnemyStats.cs
    ├── Managers/
    │   ├── WaveManager.cs
    │   └── LevelUpManager.cs
    └── UI/
        └── UIManager.cs

🧠 Script Kullanımları
🎯 PlayerController.cs

Karakterin otomatik saldırı sistemini yönetir.

Önemli değişkenler:

attackRange → Saldırı menzili (varsayılan: 1)

attackCooldown → Saldırı aralığı (varsayılan: 0.5 sn)

attackDamage → Hasar miktarı (varsayılan: 10)

enemyLayer → Enemy layer referansı

Kullanım:

Player objesine eklenir

enemyLayer → Enemy olarak ayarlanır

Otomatik çalışır

❤️ PlayerStats.cs

Karakterin istatistiklerini yönetir.

Değişkenler:

maxHP, currentHP

currentXP, currentLevel

xpToNextLevel

armor

Metodlar:

TakeDamage(int damage)

GainXP(int amount)

HealHP(int amount)

IncreaseMaxHP(int amount)

IncreaseArmor(int amount)

👾 EnemyController.cs

Düşman hareketi ve saldırısını yönetir.

Değişkenler:

attackInterval (varsayılan: 1.5 sn)

stopDistance (varsayılan: 1.5)

Kullanım:

Enemy ve Boss prefablarına eklenir

Player’a otomatik yönelir ve saldırır

💀 EnemyStats.cs

Düşman istatistiklerini yönetir.

Değerler:

Normal Enemy → HP: 30 | Damage: 5 | XP: 20 | Speed: 2

Boss → HP: 200 | Damage: 15 | XP: 100 | Speed: 1.5

Metodlar:

TakeDamage(int damageAmount)

Die()

🌊 WaveManager.cs

Dalga sistemini kontrol eder.

Değişkenler:

leftSpawnPoint, rightSpawnPoint

normalEnemyPrefab, bossPrefab

totalWaves (5)

timeBetweenWaves (3 sn)

baseEnemyCount (3)

enemyHPMultiplier (1.3)

enemySpeedMultiplier (1.1)

Metodlar:

OnEnemyDied()

GetCurrentWave()

GetTotalWaves()

🧩 LevelUpManager.cs

Yetenek seçim sistemini yönetir.

Görevleri:

Oyunu duraklatır

3 rastgele yetenek sunar

Seçilen yeteneği uygular

Metodlar:

ShowLevelUpPanel()

SelectAbility1()

SelectAbility2()

SelectAbility3()

🖥️ UIManager.cs

Oyun arayüzünü yönetir.

Bileşenler:

HP Bar

XP Bar

Wave Text

Level Text

Stage Clear Panel

Game Over Panel

Metodlar:

ShowStageClear()

ShowGameOver()

🎮 Oyun Nasıl Oynanır

Oyun başladığında karakter ekranın ortasında yer alır

Düşmanlar sağdan ve soldan gelir

Karakter otomatik olarak saldırır

Düşmanlar öldükçe XP kazanılır

Seviye atlandığında yetenek seçilir

Tüm dalgalar tamamlanır

Boss ortaya çıkar

Boss yenilince bölüm tamamlanır

📌 Notlar

Oyun mobil için tasarlanmıştır ancak geliştirme PC üzerinde yapılmıştır

Tüm sistemler modüler şekilde geliştirilmiştir

Kod yapısı OOP prensiplerine uygundur
