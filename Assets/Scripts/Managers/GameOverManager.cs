using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    // Hoca Sorarsa: "Oyun bitince statları sıfırlayıp ilgili sahneyi yükleyen merkezi fonksiyonumuz."
    public void RestartGame()
    {
        ResetAndLoad("WeaponSelect");
    }

    public void GoToMainMenu()
    {
        ResetAndLoad("MainMenu");
    }

    // Kod tekrarını önlemek için ortak bir fonksiyon yazdık (Clean Code prensibi)
    private void ResetAndLoad(string sceneName)
    {
        if (FloorManager.Instance != null)
            FloorManager.Instance.currentFloor = 0; // Katı sıfırla

        Time.timeScale = 1f; // Hoca Sorarsa: "Ölünce zaman durmuş olabilir, yeni sahnede bug olmasın diye 1'e eşitliyoruz."
        SceneManager.LoadScene(sceneName);
    }
}