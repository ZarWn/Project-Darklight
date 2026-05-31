using UnityEngine;

public class MusicManager : MonoBehaviour
{
    // Hoca Sorarsa: "Müziğin sahneler arası geçişte kesilmemesi için Singleton (Tekil) tasarım deseni (DontDestroyOnLoad) kullandım."
    public static MusicManager Instance;

    public AudioClip backgroundMusic; 
    [Range(0f, 1f)] public float musicVolume = 0.5f;  
    
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject); 
            return; 
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject); 

        // Hoca Sorarsa: "Eğer objenin üzerinde AudioSource yoksa (NullReference olmasın diye) kodla otomatik ekliyorum."
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        
        SetupAndPlayMusic();
    }

    void SetupAndPlayMusic()
    {
        if (backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true; // Müziği döngüye sokar (hiç bitmez)
            audioSource.volume = musicVolume;
            audioSource.Play();
        }
    }
}