using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject settingsPanel;

    public void Play()
    {
        // Warning: "Gameplay" es un string mágico, si renombrás la escena se rompe sin error de compilación. Usá una const o un campo serializado.
        SceneManager.LoadScene("Gameplay");
    }

    public void OpenSettings()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public void Exit()
    {
        Debug.Log("Exit"); 
        Application.Quit();
    }
}