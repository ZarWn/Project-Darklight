# 🎮 Wave Crusher

Wave Crusher, Unity 6 ile geliştirilmiş 2D mobil aksiyon türünde bir oyundur.
Oyuncu ekranın ortasında sabit duran bir karakteri yönetir. Karakter otomatik 
olarak sağa ve sola saldırır. Düşmanlar dalgalar halinde sağdan ve soldan gelir. 
Oyuncu düşman öldürerek XP kazanır, seviye atlar ve yeni yetenekler seçer. 
Son dalgada Boss düşmanını öldürerek bölümü tamamlar.

## 📱 Platform
- Geliştirme: PC (Windows)
- Hedef Platform: Android
- Geliştirme Motoru: Unity 6 (6000.3.9f1)
- Programlama Dili: C#

## 🎯 Oyun Özellikleri
- Otomatik saldırı sistemi
- Dalga dalga gelen düşmanlar
- Her dalgada güçlenen düşmanlar
- 10 farklı yetenek seçim sistemi
- 3 Bölge ve 9 Stage
- 3 Fazlı Boss sistemi
- Boss uyarı sistemi
- HP, XP ve seviye sistemi
- Zırh sistemi
- Ana menü ve Game Over ekranı

## 🛠️ Kullanılan Teknolojiler
- Unity 6 (6000.3.9f1)
- C#
- Unity Input System
- Unity UI (TextMeshPro)
- Unity SceneManagement

<pre>
- ## 📥 Kurulum ve Çalıştırma

1. Bu repoyu bilgisayarınıza klonlayın: git clone https://github.com/kullaniciadi/wavecrusher.git
2. Unity Hub'ı açın.
3. Unity Hub'da "Open" butonuna tıklayın.
4. Klonladığınız klasörü seçin.
5. Unity 6 (6000.3.9f1) sürümü ile projeyi açın.
6. Assets/Scenes klasöründen "MainMenu" sahnesini açın.
7. Unity editöründe Play butonuna basarak oyunu çalıştırın.

## 📁 Proje Klasör Yapısı
Assets/
├── Prefabs/
│ ├── Enemy.prefab
│ └── Boss.prefab
├── Scenes/
│ ├── MainMenu.unity
│ └── GameScene.unity
├── Sprites/
└── Scripts/
├── Player/
│ ├── PlayerController.cs
│ └── PlayerStats.cs
├── Enemy/
│ ├── EnemyController.cs
│ ├── EnemyStats.cs
│ └── BossController.cs
├── Managers/
│ ├── WaveManager.cs
│ ├── StageManager.cs
│ ├── LevelUpManager.cs
│ ├── GameOverManager.cs
│ └── MainMenuManager.cs
└── UI/
└── UIManager.cs </pre>


## 🎮 Oyun Nasıl Oynanır

1. Ana menüden Oyna butonuna bas.
2. Oyun başladığında karakter ekranın ortasına yerleşir.
3. Düşmanlar sağdan ve soldan gelmeye başlar.
4. Karakter otomatik olarak saldırır, herhangi bir tuşa basmaya gerek yoktur.
5. Düşmanlar öldürüldükçe XP kazanılır.
6. Yeterli XP birikince seviye atlanır ve yetenek seçim ekranı açılır.
7. 3 yetenek kartından biri seçilir ve oyun devam eder.
8. Tüm dalgalar tamamlanınca Boss uyarısı çıkar.
9. Boss öldürülünce bir sonraki stage'e geçilir.
10. 3 bölge ve 9 stage tamamlanınca oyun biter.

## 📜 Scriptlerin Detaylı Açıklaması

### 1. PlayerController.cs
Karakterin otomatik saldırı sistemini yönetir. Player objesine eklenir.

#### Değişkenler:
| Değişken | Tip | Açıklama | Varsayılan Değer |
|---|---|---|---|
| attackRange | float | Saldırı menzili | 1 |
| attackCooldown | float | Saldırılar arası bekleme süresi | 0.5 |
| attackDamage | int | Her vuruşta verilen hasar | 10 |
| enemyLayer | LayerMask | Düşman layer'ı | Enemy |

#### Metodlar:
```csharp
// Otomatik saldırı sistemi, her attackCooldown saniyede bir çalışır
void AutoAttack()

// Belirtilen yönde saldırı gerçekleştirir, 1 sağ -1 sol
bool PerformAttack(int direction)

// Yetenek sistemi tarafından çağrılır, saldırı hızını artırır
public void IncreaseAttackSpeed(float amount)

// Yetenek sistemi tarafından çağrılır, saldırı hasarını artırır
public void IncreaseAttackDamage(int amount)

// Yetenek sistemi tarafından çağrılır, saldırı menzilini artırır
public void IncreaseAttackRange(float amount)

// Yetenek sistemi tarafından çağrılır, ateş hasarı ekler
public void IncreaseFireDamage(int amount)

// Yetenek sistemi tarafından çağrılır, saldırı hızını maksimuma çıkarır
public void ActivateSuperSpeed()
```

#### Kullanımı:
Player objesine bu scripti ekleyin. Inspector'dan enemyLayer değerini 
Enemy olarak ayarlayın. Script otomatik olarak çalışır, ek bir ayar gerekmez.

---

### 2. PlayerStats.cs
Karakterin tüm istatistiklerini yönetir. Player objesine eklenir.

#### Değişkenler:
| Değişken | Tip | Açıklama | Varsayılan Değer |
|---|---|---|---|
| maxHP | int | Maksimum can puanı | 100 |
| currentHP | int | Mevcut can puanı | 100 |
| currentXP | int | Mevcut XP puanı | 0 |
| currentLevel | int | Mevcut seviye | 1 |
| xpToNextLevel | int | Sonraki seviye için gereken XP | 100 |
| armor | int | Zırh değeri | 0 |

#### Metodlar:
```csharp
// Karaktere hasar verir, zırh değeri hesaba katılır
public void TakeDamage(int damage)

// Karaktere XP kazandırır, yeterli XP birikince seviye atlar
public void GainXP(int amount)

// Karakterin canını yeniler
public void HealHP(int amount)

// Karakterin maksimum canını artırır
public void IncreaseMaxHP(int amount)

// Karakterin zırhını artırır
public void IncreaseArmor(int amount)

// HP yüzdesini döndürür (0-1 arası)
public float GetHPPercent()

// XP yüzdesini döndürür (0-1 arası)
public float GetXPPercent()
```

#### Hasar Hesaplama Formülü:
```
Alınan Gerçek Hasar = Düşman Hasarı - Oyuncu Zırhı (minimum 1)
```

#### Seviye Atlama Sistemi:
```
Her seviyede gereken XP = Önceki XP x 1.2
Örnek:
Seviye 1 → 2: 100 XP
Seviye 2 → 3: 120 XP
Seviye 3 → 4: 144 XP
```

### 3. EnemyStats.cs
Düşmanın istatistiklerini yönetir. Enemy ve Boss prefablarına eklenir.

#### Değişkenler:
| Değişken | Tip | Açıklama | Varsayılan Değer |
|---|---|---|---|
| maxHP | int | Maksimum can puanı | 30 |
| currentHP | int | Mevcut can puanı | 30 |
| damage | int | Oyuncuya verilen hasar | 5 |
| xpReward | int | Öldürünce verilen XP | 20 |
| moveSpeed | float | Hareket hızı | 2 |

#### Metodlar:
```csharp
// Düşmana hasar verir, can sıfırlanınca Die çağrılır
public void TakeDamage(int damageAmount)

// Düşmanı öldürür, oyuncuya XP verir, WaveManager'a bildirir
void Die()

// HP yüzdesini döndürür (0-1 arası)
public float GetHPPercent()
```

#### Dalga Güçlendirme Sistemi:
```
Her dalgada HP artışı = Başlangıç HP x (1.3 ^ dalga numarası)
Her dalgada Hız artışı = Başlangıç Hız x (1.1 ^ dalga numarası)
Her dalgada XP artışı = Başlangıç XP x (1 + dalga numarası x 0.2)
```

---

### 4. EnemyController.cs
Düşmanın hareketini ve saldırısını yönetir. Enemy prefabına eklenir.

#### Değişkenler:
| Değişken | Tip | Açıklama | Varsayılan Değer |
|---|---|---|---|
| attackInterval | float | Saldırılar arası bekleme süresi | 1.5 |
| stopDistance | float | Durma mesafesi | 1.5 |

#### Metodlar:
```csharp
// Düşmanı oyuncuya doğru hareket ettirir
void MoveTowardsPlayer()

// Düşmanın oyuncuya saldırmasını yönetir
void AttackPlayer()
```

#### Hareket Sistemi:
```
Düşman spawn noktasından çıkar
        ↓
Oyuncuya doğru X ekseninde hareket eder
        ↓
stopDistance mesafesine gelince durur
        ↓
attackInterval sürede bir saldırır
```

---

### 5. BossController.cs
Boss düşmanının hareketini, saldırısını ve fazlarını yönetir. Boss prefabına eklenir.

#### Değişkenler:
| Değişken | Tip | Açıklama | Varsayılan Değer |
|---|---|---|---|
| stopDistance | float | Durma mesafesi | 1.5 |
| normalAttackInterval | float | Faz 1 saldırı aralığı | 2 |
| phase2AttackInterval | float | Faz 2 saldırı aralığı | 1.2 |
| phase3AttackInterval | float | Faz 3 saldırı aralığı | 0.8 |
| aoeRange | float | Alan saldırısı menzili | 3 |
| aoeDamage | int | Alan saldırısı hasarı | 20 |
| aoeInterval | float | Alan saldırısı aralığı | 5 |
| normalSpeed | float | Faz 1 hareket hızı | 1.5 |
| phase2Speed | float | Faz 2 hareket hızı | 2.5 |
| phase3Speed | float | Faz 3 hareket hızı | 3.5 |

#### Metodlar:
```csharp
// Boss fazını kontrol eder ve gerekirse faz değiştirir
void CheckPhase()

// Boss hareketini yönetir
void HandleMovement()

// Normal saldırıyı yönetir
void HandleAttack()

// Alan saldırısını yönetir (sadece Faz 3)
void HandleAOE()

// Boss HP barını günceller
void UpdateBossHP()

// Bölgeye göre boss adını döndürür
string GetBossName()
```

#### Boss Faz Sistemi:
```
Faz 1 (HP %100 - %60):
- Normal hız: 1.5
- Saldırı aralığı: 2 saniye
- Alan saldırısı: Yok

Faz 2 (HP %60 - %30):
- Hız: 2.5
- Saldırı aralığı: 1.2 saniye
- Alan saldırısı: Yok

Faz 3 (HP %30 - %0):
- Hız: 3.5
- Saldırı aralığı: 0.8 saniye
- Alan saldırısı: Her 5 saniyede bir, 3 birim menzil, 20 hasar

Boss İsimleri:
Bölge 1: Orman Canavarı
Bölge 2: Çöl Ejderi
Bölge 3: Karanlık Kral
```

### 6. WaveManager.cs
Oyunun dalga sistemini yönetir. Ayrı bir WaveManager objesine eklenir.

#### Değişkenler:
| Değişken | Tip | Açıklama | Varsayılan Değer |
|---|---|---|---|
| leftSpawnPoint | Transform | Sol spawn noktası | - |
| rightSpawnPoint | Transform | Sağ spawn noktası | - |
| normalEnemyPrefab | GameObject | Normal düşman prefabı | - |
| bossPrefab | GameObject | Boss prefabı | - |
| totalWaves | int | Toplam dalga sayısı | 5 |
| timeBetweenWaves | float | Dalgalar arası bekleme | 3 |
| spawnInterval | float | Düşmanlar arası spawn süresi | 0.8 |
| baseEnemyCount | int | İlk dalgadaki düşman sayısı | 3 |
| enemyHPMultiplier | float | Her dalgada HP artış çarpanı | 1.3 |
| enemySpeedMultiplier | float | Her dalgada hız artış çarpanı | 1.1 |

#### Metodlar:
```csharp
// Dalga sistemini başlatır
IEnumerator StartNextWave()

// Belirtilen dalgayı spawn eder
IEnumerator SpawnWave(int waveNumber)

// Düşman spawn eder ve güçlendirir
void SpawnEnemy(Transform spawnPoint, int waveNumber)

// Boss spawn eder, uyarı gösterir
IEnumerator SpawnBoss()

// Düşman öldüğünde EnemyStats tarafından çağrılır
public void OnEnemyDied()

// Stage Clear işlemlerini yapar
void StageClear()

// Sonraki sahneyi yükler
IEnumerator LoadNextStage()

// Mevcut dalga numarasını döndürür
public int GetCurrentWave()

// Toplam dalga sayısını döndürür
public int GetTotalWaves()
```

#### Dalga Sistemi Akışı:
```
Oyun Başlar (2 saniye bekleme)
        ↓
Dalga 1 Başlar
        ↓
Düşmanlar sırayla sağdan ve soldan spawn olur
        ↓
Tüm düşmanlar ölünce dalga tamamlanır
        ↓
3 saniye bekleme
        ↓
Sonraki Dalga (daha fazla ve güçlü düşman)
        ↓
Tüm dalgalar tamamlanınca Boss Uyarısı
        ↓
Boss Spawn
        ↓
Boss Ölünce Stage Clear
```

---

### 7. StageManager.cs
Oyunun bölge ve stage sistemini yönetir. DontDestroyOnLoad ile sahneler arası korunur.

#### Değişkenler:
| Değişken | Tip | Açıklama | Varsayılan Değer |
|---|---|---|---|
| currentRegion | int | Mevcut bölge | 1 |
| currentStage | int | Mevcut stage | 1 |
| totalRegions | int | Toplam bölge sayısı | 3 |
| stagesPerRegion | int | Bölge başına stage sayısı | 3 |
| regionHPMultiplier | float | Bölge HP artış çarpanı | 1.5 |
| regionSpeedMultiplier | float | Bölge hız artış çarpanı | 1.2 |
| regionDamageMultiplier | float | Bölge hasar artış çarpanı | 1.3 |
| wavesPerStage | int[] | Her stage için dalga sayısı | {5, 6, 7} |

#### Metodlar:
```csharp
// Sonraki stage'e geçer, gerekirse bölge değiştirir
public void NextStage()

// Sonraki bölgeye geçer
void NextRegion()

// Oyun tamamlandığında çağrılır
void GameCompleted()

// Mevcut stage için dalga sayısını döndürür
public int GetWavesForCurrentStage()

// Bölge HP çarpanını döndürür
public float GetRegionHPMultiplier()

// Bölge hız çarpanını döndürür
public float GetRegionSpeedMultiplier()

// Bölge hasar çarpanını döndürür
public float GetRegionDamageMultiplier()

// Bölge adını döndürür
public string GetRegionName()

// Bölge ve stage bilgisini döndürür
public string GetStageInfo()
```

#### Bölge ve Stage Yapısı:
```
Bölge 1 - Orman:
├── Stage 1: 5 Dalga + Boss
├── Stage 2: 6 Dalga + Boss
└── Stage 3: 7 Dalga + Boss

Bölge 2 - Çöl (1.5x güçlü):
├── Stage 1: 5 Dalga + Boss
├── Stage 2: 6 Dalga + Boss
└── Stage 3: 7 Dalga + Boss

Bölge 3 - Karanlık Kale (2.25x güçlü):
├── Stage 1: 5 Dalga + Boss
├── Stage 2: 6 Dalga + Boss
└── Stage 3: 7 Dalga + Boss
```

---

### 8. LevelUpManager.cs
Yetenek seçim sistemini yönetir. Ayrı bir LevelUpManager objesine eklenir.

#### Değişkenler:
| Değişken | Tip | Açıklama |
|---|---|---|
| levelUpPanel | GameObject | Yetenek seçim paneli |
| card1Title | TextMeshProUGUI | Birinci kart başlığı |
| card2Title | TextMeshProUGUI | İkinci kart başlığı |
| card3Title | TextMeshProUGUI | Üçüncü kart başlığı |
| card1Desc | TextMeshProUGUI | Birinci kart açıklaması |
| card2Desc | TextMeshProUGUI | İkinci kart açıklaması |
| card3Desc | TextMeshProUGUI | Üçüncü kart açıklaması |
| card1Icon | TextMeshProUGUI | Birinci kart ikonu |
| card2Icon | TextMeshProUGUI | İkinci kart ikonu |
| card3Icon | TextMeshProUGUI | Üçüncü kart ikonu |
| playerController | PlayerController | Player controller referansı |
| playerStats | PlayerStats | Player stats referansı |

#### Metodlar:
```csharp
// Oyunu duraklatır ve yetenek panelini açar
public void ShowLevelUpPanel()

// 3 farklı rastgele yetenek seçer
void SelectRandomAbilities()

// Birinci kartı seçer
public void SelectAbility1()

// İkinci kartı seçer
public void SelectAbility2()

// Üçüncü kartı seçer
public void SelectAbility3()

// Seçilen yeteneği uygular
void ApplyAbility(int index)

// Paneli kapatır ve oyunu devam ettirir
void HideLevelUpPanel()
```

#### Yetenek Listesi:
```
1.  Saldırı Hasarı  → +5 hasar
2.  Saldırı Hızı    → -0.1 cooldown
3.  Saldırı Menzili → +0.5 menzil
4.  Can Yenileme    → +10 HP
5.  Çift Hasar      → x2 hasar
6.  Zırh            → +2 zırh
7.  Ateş Hasarı     → +3 hasar
8.  Max Can Artışı  → +20 max HP
9.  Güçlü Darbe     → +10 hasar
10. Süper Hız       → -0.2 cooldown
```

---

### 9. GameOverManager.cs
Game Over ekranındaki butonların işlevlerini yönetir.

#### Metodlar:
```csharp
// StageManager sıfırlar ve oyunu baştan başlatır
public void RestartGame()

// StageManager sıfırlar ve ana menüye döner
public void GoToMainMenu()
```

---

### 10. MainMenuManager.cs
Ana menü butonlarının işlevlerini yönetir.

#### Metodlar:
```csharp
// GameScene sahnesini yükler
public void PlayGame()

// Oyundan çıkış yapar
public void QuitGame()
```

---

### 11. UIManager.cs
Oyunun tüm kullanıcı arayüzünü yönetir.

#### Değişkenler:
| Değişken | Tip | Açıklama |
|---|---|---|
| hpBar | Slider | HP barı |
| xpBar | Slider | XP barı |
| waveText | TextMeshProUGUI | Dalga sayacı |
| levelText | TextMeshProUGUI | Seviye göstergesi |
| stageInfoText | TextMeshProUGUI | Bölge ve stage bilgisi |
| stageClearPanel | GameObject | Stage Clear paneli |
| gameOverPanel | GameObject | Game Over paneli |
| bossHPPanel | GameObject | Boss HP paneli |
| bossHPBar | Slider | Boss HP barı |
| bossNameText | TextMeshProUGUI | Boss adı |
| bossWarningPanel | GameObject | Boss uyarı paneli |
| playerStats | PlayerStats | Player stats referansı |
| waveManager | WaveManager | Wave manager referansı |

#### Metodlar:
```csharp
// HP barını günceller
void UpdateHP()

// XP barını günceller
void UpdateXP()

// Dalga sayacını günceller
void UpdateWaveText()

// Seviye göstergesini günceller
void UpdateLevelText()

// Stage bilgisini günceller
void UpdateStageInfo()

// Stage Clear panelini gösterir
public void ShowStageClear()

// Game Over panelini gösterir
public void ShowGameOver()

// Boss HP panelini gösterir
public void ShowBossHP(int maxHP, string bossName)

// Boss HP barını günceller
public void UpdateBossHP(int currentHP)

// Boss HP panelini gizler
public void HideBossHP()

// Boss uyarısını gösterir
public void ShowBossWarning()
```

<pre>
    ## 🖥️ UI Elemanları

### GameScene UI Yapısı:
```
Canvas
├── HPBar (Sol üst - Kırmızı)
├── XPBar (Sol üst - Sarı)
├── HPText (HP etiketi)
├── XPText (XP etiketi)
├── LevelText (Seviye göstergesi)
├── WaveText (Dalga sayacı)
├── StageInfoText (Bölge ve stage bilgisi)
├── LevelUpPanel (Yetenek seçim ekranı)
│   ├── LevelUpTitle
│   ├── Card1
│   │   ├── Card1Icon
│   │   ├── Card1Title
│   │   ├── Card1Desc
│   │   └── Card1Button
│   ├── Card2
│   │   ├── Card2Icon
│   │   ├── Card2Title
│   │   ├── Card2Desc
│   │   └── Card2Button
│   └── Card3
│       ├── Card3Icon
│       ├── Card3Title
│       ├── Card3Desc
│       └── Card3Button
├── StageClearPanel
├── GameOverPanel
│   ├── GameOverText
│   ├── RestartButton
│   └── MainMenuButton
├── BossHPPanel
│   ├── BossNameText
│   └── BossHPBar
└── BossWarningPanel
    └── BossWarningText
```

### MainMenu UI Yapısı:
```
Canvas
└── MainMenuPanel
    ├── GameTitle
    ├── PlayButton
    └── QuitButton
```

## ⚙️ Inspector Ayarları

### Player Objesi:
```
Tag: Player
Layer: Default
Bileşenler:
- Sprite Renderer (Mavi renk)
- Rigidbody 2D (Kinematic, Freeze Rotation Z)
- Capsule Collider 2D (Size: 0.8, 1)
- PlayerController
  - Attack Range: 1
  - Attack Cooldown: 0.5
  - Attack Damage: 10
  - Enemy Layer: Enemy
- PlayerStats
  - Max HP: 100
  - XP To Next Level: 100
```

### Enemy Prefabı:
```
Tag: Untagged
Layer: Enemy
Bileşenler:
- Sprite Renderer (Kırmızı renk)
- Rigidbody 2D (Kinematic, Freeze Rotation Z)
- Capsule Collider 2D (Size: 0.8, 1)
- EnemyStats
  - Max HP: 30
  - Damage: 5
  - XP Reward: 20
  - Move Speed: 2
- EnemyController
  - Attack Interval: 1.5
  - Stop Distance: 1.5
```

### Boss Prefabı:
```
Tag: Untagged
Layer: Enemy
Bileşenler:
- Sprite Renderer (Mor renk, Scale: 2x2)
- Rigidbody 2D (Kinematic, Freeze Rotation Z)
- Capsule Collider 2D (Size: 1.5, 2)
- EnemyStats
  - Max HP: 200
  - Damage: 15
  - XP Reward: 100
  - Move Speed: 1.5
- BossController
  - Stop Distance: 1.5
  - Normal Attack Interval: 2
  - Phase2 Attack Interval: 1.2
  - Phase3 Attack Interval: 0.8
  - AOE Range: 3
  - AOE Damage: 20
  - AOE Interval: 5
  - Normal Speed: 1.5
  - Phase2 Speed: 2.5
  - Phase3 Speed: 3.5
```

### WaveManager Objesi:
```
Bileşenler:
- WaveManager
  - Left Spawn Point: LeftSpawn (-9, 0, 0)
  - Right Spawn Point: RightSpawn (9, 0, 0)
  - Normal Enemy Prefab: Enemy
  - Boss Prefab: Boss
  - Total Waves: 5
  - Time Between Waves: 3
  - Spawn Interval: 0.8
  - Base Enemy Count: 3
  - Enemy HP Multiplier: 1.3
  - Enemy Speed Multiplier: 1.1
```

### StageManager Objesi:
```
Bileşenler:
- StageManager
  - Current Region: 1
  - Current Stage: 1
  - Total Regions: 3
  - Stages Per Region: 3
  - Region HP Multiplier: 1.5
  - Region Speed Multiplier: 1.2
  - Region Damage Multiplier: 1.3
  - Waves Per Stage: [5, 6, 7]
```

## 🗺️ Sahne Yapısı

### Build Settings Sıralaması:
```
0: Scenes/MainMenu
1: Scenes/GameScene
```

### GameScene Hierarchy:
```
Main Camera
Player
LeftSpawn
RightSpawn
WaveManager
StageManager
LevelUpManager
UIManager
GameOverManager
Canvas
EventSystem
```
</pre>
