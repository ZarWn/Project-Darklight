using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public void RestartGame()
    {
        
        if (StageManager.Instance != null)
        {
            StageManager.Instance.currentRegion = 1;
            StageManager.Instance.currentStage = 1;
        }

        
        Time.timeScale = 1f;

        
        SceneManager.LoadScene("GameScene");
        Debug.Log("Oyun yeniden başlatıldı!");
    }

    public void GoToMainMenu()
    {
        
        if (StageManager.Instance != null)
        {
            StageManager.Instance.currentRegion = 1;
            StageManager.Instance.currentStage = 1;
        }

        
        Time.timeScale = 1f;

        
        SceneManager.LoadScene("MainMenu");
        Debug.Log("Ana menüye dönüldü!");
    }
}