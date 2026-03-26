using UnityEngine;

public class CurrencyController : MonoBehaviour
{
    private static CurrencyController _instance;
    public static CurrencyController Instance => _instance;

    private float score;

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

        if (!PlayerPrefs.HasKey("Score"))
        {
            PlayerPrefs.SetFloat("Score", 300f);
            PlayerPrefs.Save();
        }
    }
    public void AddScore(float amount)
    {
        score += amount;
    }
    public void SaveScore()
    {
        PlayerPrefs.SetFloat("Score", score);
        PlayerPrefs.Save();
    }
    public float GetScore(){ return score; }
}
