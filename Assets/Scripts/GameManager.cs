using UnityEngine;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        Time.timeScale = 1f;
        AudioManager.Instance.PlayGameMusic();
        AudioManager.Instance.PitchRegular();
    }
}
