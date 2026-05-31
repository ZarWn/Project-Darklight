using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Hoca Sorarsa: "Oyunun giriş noktası. SceneManager kütüphanesini kullanarak sahne indekslemesi (isimle) yapıyorum."
    public void PlayGame()
    {
        SceneManager.LoadScene("WeaponSelect");
    }

    public void QuitGame()
    {
        // Hoca Sorarsa: "Application.Quit() sadece build alınmış (exe/apk) versiyonda çalışır, Unity editöründe çalışmaz."
        Application.Quit();
    }
}