using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuController : MonoBehaviour
{
    [Header("Pause Settings")]
    private bool _pauseGame = false;
    [SerializeField]
    private GameObject _pausePanel;
    public void ChangeScene( string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void OnPauseGame(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if(_pauseGame)
                ResumeGame();
            else
                OnPauseGame();
        }
    }

    private void OnPauseGame()
    {
        Time.timeScale = 0f;
        _pausePanel.SetActive(true);
        _pauseGame = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        _pausePanel.SetActive(false);
        _pauseGame = false;
    }
}
