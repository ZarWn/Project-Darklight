using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Müzik Ayarları")]
    public AudioClip backgroundMusic; // Her sahnede çalacak o tek müzik dosyası
    [Range(0f, 1f)]
    public float musicVolume = 0.5f;  // Müziğin ses seviyesi
    
    private AudioSource audioSource;

    private void Awake()
    {
        // --- SINGLETON & DONT_DESTROY_ON_LOAD ---
        // Eğer bu objeden sahnede zaten varsa yenisini yok et, yoksa kalıcı yap
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Sahneler değişse bile bu objeyi silme!
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Hoparlör bileşenini ayarla
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Müziği döngüsel (Loop) olarak başlat
        SetupAndPlayMusic();
    }

    void SetupAndPlayMusic()
    {
        if (backgroundMusic != null && audioSource != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true;       // Müziğin hiç durmadan sürekli dönmesini sağlar
            audioSource.volume = musicVolume;
            audioSource.playOnAwake = false;
            audioSource.Play();
            
            Debug.Log("Arka plan müziği kesintisiz çalmak üzere başlatıldı.");
        }
    }

    // İleride seçenekler menüsünden sesi kısmak istersen kullanabileceğin fonksiyon
    public void SetVolume(float volume)
    {
        musicVolume = volume;
        if (audioSource != null)
        {
            audioSource.volume = musicVolume;
        }
    }
}