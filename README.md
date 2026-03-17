Wave Crusher
Wave Crusher, Unity 6 ile geliştirilmiş 2D mobil aksiyon türünde bir oyundur. Oyuncu ekranın ortasında sabit duran bir karakteri yönetir. Karakter otomatik olarak sağa ve sola saldırır. Düşmanlar dalgalar halinde sağdan ve soldan gelir. Oyuncu düşman öldürerek XP kazanır, seviye atlar ve yeni yetenekler seçer. Son dalgada Boss düşmanını öldürerek bölümü tamamlar.

Oyun Özellikleri
Otomatik saldırı sistemi
Dalga dalga gelen düşmanlar
Her dalgada güçlenen düşmanlar
10 farklı yetenek seçim sistemi
Boss dalgası
HP, XP ve seviye sistemi
Zırh sistemi
Kullanılan Teknolojiler
Unity 6 (6000.3.9f1)
C#
Unity Input System
Unity UI (TextMeshPro)
Kurulum ve Çalıştırma
Bu repoyu bilgisayarınıza klonlayın:
text

git clone https://github.com/kullaniciadi/wavecrusher.git
Unity Hub'ı açın.

Unity Hub'da "Open" butonuna tıklayın.

Klonladığınız klasörü seçin.

Unity 6 (6000.3.9f1) sürümü ile projeyi açın.

Assets/Scenes klasöründen "GameScene" sahnesini açın.

Unity editöründe Play butonuna basarak oyunu çalıştırın.

Proje Klasör Yapısı
text

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
        
Scriptlerin Kullanımı
PlayerController.cs
Bu script karakterin otomatik saldırı sistemini yönetir. Player objesine eklenir.

Önemli değişkenler:

attackRange: Saldırı menzilini belirler. Varsayılan değer 1'dir.
attackCooldown: İki saldırı arasındaki bekleme süresidir. Varsayılan değer 0.5 saniyedir.
attackDamage: Her vuruşta verilen hasardır. Varsayılan değer 10'dur.
enemyLayer: Düşman layerını belirtir. Inspector'dan Enemy layerı seçilmelidir.

Kullanımı:
Player objesine bu scripti ekleyin. Inspector'dan enemyLayer değerini Enemy olarak ayarlayın. Script otomatik olarak çalışır, ek bir ayar gerekmez.

PlayerStats.cs
Bu script karakterin istatistiklerini yönetir. Player objesine eklenir.

Önemli değişkenler:

maxHP: Karakterin maksimum can puanıdır. Varsayılan değer 100'dür.
currentHP: Karakterin mevcut can puanıdır.
currentXP: Karakterin mevcut XP puanıdır.
currentLevel: Karakterin mevcut seviyesidir.
xpToNextLevel: Sonraki seviye için gereken XP miktarıdır. Varsayılan değer 100'dür.
armor: Karakterin zırh değeridir. Düşman hasarını azaltır.

Önemli metodlar:

TakeDamage(int damage): Karaktere hasar verir. Zırh değeri hesaba katılır.
GainXP(int amount): Karaktere XP kazandırır. Yeterli XP birikince otomatik seviye atlar.
HealHP(int amount): Karakterin canını yeniler.
IncreaseMaxHP(int amount): Karakterin maksimum canını artırır.
IncreaseArmor(int amount): Karakterin zırhını artırır.

EnemyController.cs
Bu script düşmanın hareketini ve saldırısını yönetir. Enemy ve Boss prefablarına eklenir.

Önemli değişkenler:

attackInterval: Düşmanın kaç saniyede bir saldıracağını belirler. Varsayılan değer 1.5 saniyedir.
stopDistance: Düşmanın karaktere ne kadar yaklaşınca duracağını belirler. Varsayılan değer 1.5'tir.

Kullanımı:
Enemy prefabına bu scripti ekleyin. Script otomatik olarak Player'ı bulur ve ona doğru hareket eder. Player'a yeterince yaklaştığında durur ve saldırmaya başlar.

EnemyStats.cs
Bu script düşmanın istatistiklerini yönetir. Enemy ve Boss prefablarına eklenir.

Önemli değişkenler:

maxHP: Düşmanın maksimum can puanıdır. Normal düşman için 30, Boss için 200'dür.
damage: Düşmanın oyuncuya verdiği hasardır. Normal düşman için 5, Boss için 15'tir.
xpReward: Düşman öldürüldüğünde verilen XP miktarıdır. Normal düşman için 20, Boss için 100'dür.
moveSpeed: Düşmanın hareket hızıdır. Normal düşman için 2, Boss için 1.5'tir.

Önemli metodlar:

TakeDamage(int damageAmount): Düşmana hasar verir. Can sıfırlanınca Die metodu çağrılır.
Die(): Düşmanı öldürür, oyuncuya XP verir ve WaveManager'a bildirim gönderir.

WaveManager.cs
Bu script oyunun dalga sistemini yönetir. Ayrı bir WaveManager objesine eklenir.

Önemli değişkenler:

leftSpawnPoint: Sol taraftaki spawn noktasıdır. Inspector'dan atanmalıdır.
rightSpawnPoint: Sağ taraftaki spawn noktasıdır. Inspector'dan atanmalıdır.
normalEnemyPrefab: Normal düşman prefabıdır. Inspector'dan atanmalıdır.
bossPrefab: Boss düşman prefabıdır. Inspector'dan atanmalıdır.
totalWaves: Toplam normal dalga sayısıdır. Varsayılan değer 5'tir.
timeBetweenWaves: Dalgalar arasındaki bekleme süresidir. Varsayılan değer 3 saniyedir.
baseEnemyCount: İlk dalgadaki düşman sayısıdır. Varsayılan değer 3'tür.
enemyHPMultiplier: Her dalgada düşman HP artış çarpanıdır. Varsayılan değer 1.3'tür.
enemySpeedMultiplier: Her dalgada düşman hız artış çarpanıdır. Varsayılan değer 1.1'dir.

Önemli metodlar:

OnEnemyDied(): Bir düşman öldüğünde EnemyStats tarafından çağrılır.
GetCurrentWave(): Mevcut dalga numarasını döndürür.
GetTotalWaves(): Toplam dalga sayısını döndürür.

LevelUpManager.cs
Bu script yetenek seçim sistemini yönetir. Ayrı bir LevelUpManager objesine eklenir.

Önemli değişkenler:

levelUpPanel: Yetenek seçim paneli GameObject'idir. Inspector'dan atanmalıdır.
card1Title, card2Title, card3Title: Kart başlık TextMeshPro bileşenleridir.
card1Desc, card2Desc, card3Desc: Kart açıklama TextMeshPro bileşenleridir.
card1Icon, card2Icon, card3Icon: Kart ikon TextMeshPro bileşenleridir.
playerController: PlayerController referansıdır. Inspector'dan atanmalıdır.
playerStats: PlayerStats referansıdır. Inspector'dan atanmalıdır.

Önemli metodlar:

ShowLevelUpPanel(): Oyunu duraklatır ve yetenek seçim panelini açar. PlayerStats tarafından çağrılır.
SelectAbility1(): Birinci kartı seçer ve yeteneği uygular.
SelectAbility2(): İkinci kartı seçer ve yeteneği uygular.
SelectAbility3(): Üçüncü kartı seçer ve yeteneği uygular.

UIManager.cs
Bu script oyunun kullanıcı arayüzünü yönetir. Ayrı bir UIManager objesine eklenir.

Önemli değişkenler:

hpBar: HP slider bileşenidir. Inspector'dan atanmalıdır.
xpBar: XP slider bileşenidir. Inspector'dan atanmalıdır.
waveText: Dalga sayacı TextMeshPro bileşenidir. Inspector'dan atanmalıdır.
levelText: Seviye göstergesi TextMeshPro bileşenidir. Inspector'dan atanmalıdır.
stageClearPanel: Stage Clear paneli GameObject'idir. Inspector'dan atanmalıdır.
gameOverPanel: Game Over paneli GameObject'idir. Inspector'dan atanmalıdır.
playerStats: PlayerStats referansıdır. Inspector'dan atanmalıdır.
waveManager: WaveManager referansıdır. Inspector'dan atanmalıdır.

Önemli metodlar:

ShowStageClear(): Stage Clear panelini gösterir. WaveManager tarafından çağrılır.
ShowGameOver(): Game Over panelini gösterir. PlayerStats tarafından çağrılır.

Oyun Nasıl Oynanır
Oyun başladığında karakter ekranın ortasına yerleşir.
Düşmanlar sağdan ve soldan gelmeye başlar.
Karakter otomatik olarak saldırır, herhangi bir tuşa basmaya gerek yoktur.
Düşmanlar öldürüldükçe XP kazanılır.
Yeterli XP birikince seviye atlanır ve yetenek seçim ekranı açılır.
3 yetenek kartından biri seçilir ve oyun devam eder.
Tüm dalgalar tamamlanınca Boss çıkar.
Boss öldürülünce bölüm tamamlanır.
