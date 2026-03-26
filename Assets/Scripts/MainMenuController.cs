using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    private GameObject _mainPanel;
    [SerializeField]
    private GameObject _settingsPanel;
    void Start()
    {
        _mainPanel.SetActive(true);
        _settingsPanel.SetActive(false);
        AudioManager.Instance.PlayMainMenuMusic();
        AudioManager.Instance.PitchRegular();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    
}
