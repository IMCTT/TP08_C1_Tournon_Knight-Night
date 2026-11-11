using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private TMP_Text gameOverScoreText;
    [SerializeField] private TMP_Text victoryScoreText;
    
    [Header("Audio")]
    [SerializeField] private AudioSource music;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip victoryClip;
    [SerializeField] private AudioClip gameOverClip;

    public void ShowGameOver(int score)
    {
        SaveHighScore(score);
        gameOverScoreText.text = "Score: " + score + "\nBest: " + SaveData.HighScore;
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f; 
        
        music.Stop();
        sfxSource.PlayOneShot(gameOverClip);
    }

    public void ShowVictory(int score)
    {
        SaveHighScore(score);
        victoryScoreText.text = "Score: " + score + "\nBest: " + SaveData.HighScore;
        victoryPanel.SetActive(true);
        Time.timeScale = 0f;
        
        
        music.Stop();
        sfxSource.PlayOneShot(victoryClip);
    }

    private void SaveHighScore(int score)
    {
        if (score > SaveData.HighScore)
        {
            SaveData.HighScore = score;
        }
    }

    
    public void Retry()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }
}