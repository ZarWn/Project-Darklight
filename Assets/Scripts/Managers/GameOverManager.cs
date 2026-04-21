using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public void RestartGame()
    {
        // FloorManager'ı sıfırla
        if (FloorManager.Instance != null)
        {
            FloorManager.Instance.currentFloor = 0;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene("WeaponSelect");
        Debug.Log("Oyun yeniden başlatıldı!");
    }

    public void GoToMainMenu()
    {
        // FloorManager'ı sıfırla
        if (FloorManager.Instance != null)
        {
            FloorManager.Instance.currentFloor = 0;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
        Debug.Log("Ana menüye dönüldü!");
    }
}