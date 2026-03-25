using UnityEngine;

public class GameManager : MonoBehaviour
{
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
