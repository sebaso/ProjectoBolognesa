using System;
using UnityEngine;

public class Minigame1 : MonoBehaviour
{
    [Header("References")]
    //[SerializeField]
    //private GameObject _toolObj;
    [SerializeField]
    private GameObject _hudMinigameCanvas;
    [SerializeField]
    private GameObject _selectArrow;
    [SerializeField]
    private Animator _keyAnimator;
    [SerializeField]
    private GameObject _failObj;

    [Header("Minigame Settings")]
    [SerializeField]
    private float _maxSpeed;
    [SerializeField]
    private float _incrementSpeed;
    [SerializeField]
    private float _decrementSpeed;
    [SerializeField]
    private float _rotationMultiplier;
    [SerializeField]
    private float _timeLimit;
    private float _timeRemaining;
    private float _currentSpeed;

    [Header("Posición de Parada (Ángulos)")]
    [SerializeField]
    private float _redStopAngle;
    [SerializeField]
    private float _yellowStopAngle;
    [SerializeField]
    private float _greenStopAngle;
    private float _winStopAngle;
    [SerializeField]
    private float _loseStopAngle;

    private bool _inGame;
    private bool _hasStarted;

    private void Start()
    {
        //_toolObj.SetActive(false);
        _hudMinigameCanvas.SetActive(false);
        _keyAnimator.Play("Idle");
        _inGame = false;
        _hasStarted = false;
        _timeRemaining = _timeLimit;
        _failObj.SetActive(false);
        //SetWinState(3);
        //StartMinigame();

    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!_hasStarted)
            {
                _inGame = true;
                _hasStarted = true;
            }

            if (_inGame)
                IncrementValue();
        }

        if (_hasStarted && _inGame)
        {
            ApplyGameRules();
            RotateArrow();
        }
    }

    private void ResetMinigame()
    {
        _currentSpeed = 0f;
        _keyAnimator.Play("Idle");
        _selectArrow.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        
    }

    // public void Activate(bool value)
    // {
    //     //_toolObj.SetActive(value);
    // }

    public void SetWinState(int value)
    {
        switch (value)
        {
            case 1:
                _winStopAngle = _greenStopAngle;
                break;
            case 2:
                _winStopAngle = _yellowStopAngle;
                break;
            case 3:
                _winStopAngle = _redStopAngle;
                break;
            default:
                _winStopAngle = _redStopAngle;
                break;
        }
    }

    public void StartMinigame()
    {
        ResetMinigame();
        
        Debug.Log("Starting Minigame");
        //Activate(true);
        _timeRemaining = _timeLimit;
        _hudMinigameCanvas.SetActive(true);
        _keyAnimator.Play("Spamming");
        _keyAnimator.gameObject.SetActive(true);
    }

    private void IncrementValue()
    {
        _currentSpeed += _incrementSpeed;
    }

    private void ApplyGameRules()
    {
        _currentSpeed -= _decrementSpeed * Time.deltaTime;

        _currentSpeed = Mathf.Max(0f, _currentSpeed);

        _timeRemaining -= Time.deltaTime;

        if (_currentSpeed >= _maxSpeed)
            Win();
        else if (_timeRemaining <= 0f)
            Lose();
    }

    private void RotateArrow()
    {
        _selectArrow.transform.Rotate(0, 0, _currentSpeed * _rotationMultiplier * Time.deltaTime);
    }

    private void Win()
    {
        Debug.Log("You win!");
        _currentSpeed = 0f;
        _selectArrow.transform.rotation = Quaternion.Euler(0f, 0f, _winStopAngle);
        _keyAnimator.Play("Idle");
        _keyAnimator.gameObject.SetActive(false);
        InspectorSheet.Instance.RevealAlcohol();
        _inGame = false;
    }

    private void Lose()
    {
        Debug.Log("You lose!");
        _currentSpeed = 0f;
        _selectArrow.transform.rotation = Quaternion.Euler(0f, 0f, _loseStopAngle);
        _keyAnimator.Play("Idle");
        _keyAnimator.gameObject.SetActive(false);
        _failObj.SetActive(true);

        _inGame = false;
    }

    public void StopMinigame()
    {
        if (_inGame) return;
        _keyAnimator.Play("Idle");
        _hudMinigameCanvas.SetActive(false);
        _failObj.SetActive(false);
        //Activate(false);
        _hasStarted = false;
        PlayerCamera.Instance.OnDisableCursor();
        GameManager.Instance.SetInMinigame(false);
    }
}