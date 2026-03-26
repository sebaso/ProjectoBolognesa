using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuController : MonoBehaviour
{
    [Header("Pause Settings")]
    private bool _pauseGame = false;
    [SerializeField]
    private GameObject _pausePanel;
    public bool IsPauseActive => _pauseGame;
    private static GameMenuController _instance;
    public static GameMenuController Instance => _instance;
    void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void ChangeScene( string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void OnESCPressed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if(_pauseGame)
                ResumeGame();
            else
            {
                if(!HUDController.Instance.IsPanelActive)
                    OnPauseGame();
            }
        }
    }

    private void OnPauseGame()
    {
        Time.timeScale = 0f;
        _pausePanel.SetActive(true);
        PlayerCamera.Instance.OnEnableCursor();
        _pauseGame = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        PlayerCamera.Instance.OnDisableCursor();
        _pausePanel.SetActive(false);
        _pauseGame = false;
    }
}
