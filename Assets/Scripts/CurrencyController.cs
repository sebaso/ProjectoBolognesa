using UnityEngine;

public class CurrencyController : MonoBehaviour
{
    private static CurrencyController _instance;
    public static CurrencyController Instance => _instance;

    [SerializeField]
    private int _startedScore = 500;
    [SerializeField]
    private int _totalScore = 0;
    [SerializeField]
    private int _successScore = 17;
    [SerializeField]
    private int _failScore = 25;
    [SerializeField]
    private int _maxScore = 1000;
    [SerializeField]
    private int _minScore = 0;
    public int TotalScore => _totalScore;
    public int MaxScore => _maxScore;

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

        PlayerPrefs.SetInt("Score", _startedScore);
        _totalScore = _startedScore;
    }
    public void AddScore()
    {
        _totalScore += _successScore;
        SaveScore();
    }
    public void SubtractScore()
    {
        _totalScore -= _failScore;
        SaveScore();
    }
    public void SaveScore()
    {
        _totalScore = Mathf.Clamp(_totalScore, _minScore, _maxScore);
        PlayerPrefs.SetFloat("Score", _totalScore);
    }
}
