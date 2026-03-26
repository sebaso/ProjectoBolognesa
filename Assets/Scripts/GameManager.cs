using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;


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
    void Start()
    {
        Time.timeScale = 1f;
        if(AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameMusic();
            AudioManager.Instance.PitchRegular();
        }
    }
}