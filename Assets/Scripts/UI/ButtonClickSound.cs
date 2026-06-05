using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GlobalButtonSound : MonoBehaviour
{
    public AudioClip clickClip; // Ses dosyanı buraya sürükle
    private AudioSource audioSource;

    void Start()
    {
        // Ses yöneticisi objesine bir AudioSource ekle
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // Sahnedeki tüm butonları bul
        Button[] buttons = Resources.FindObjectsOfTypeAll<Button>();
        
        foreach (Button btn in buttons)
        {
            // Butona tıklandığında tetiklenecek fonksiyonu kodla ekle
            btn.onClick.AddListener(() => PlaySound());
        }
    }

    void PlaySound()
    {
        if (clickClip != null)
        audioSource.PlayOneShot(clickClip, 0.2f);
    }
}