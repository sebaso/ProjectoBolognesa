using System.Collections;
using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    [SerializeField] private float _lerpSpeed = 2f;
    [SerializeField] private float _autoCloseDelay = 3f;
    
    private Vector3 _initialScale;
    private Coroutine _currentCoroutine;
    public bool IsFullyOpen { get; private set; }

    void Awake()
    {
        _initialScale = transform.localScale;
    }

    public void OpenDoor()
    {
        if (_currentCoroutine != null) StopCoroutine(_currentCoroutine);
        _currentCoroutine = StartCoroutine(LerpScale(0f));
    }

    public void CloseDoor()
    {
        IsFullyOpen = false;
        if (_currentCoroutine != null) StopCoroutine(_currentCoroutine);
        _currentCoroutine = StartCoroutine(LerpScale(_initialScale.x));
    }

    private IEnumerator LerpScale(float targetX)
    {
        Vector3 targetScale = new Vector3(targetX, _initialScale.y, _initialScale.z);
        while (Mathf.Abs(transform.localScale.x - targetX) > 0.01f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * _lerpSpeed);
            yield return null;
        }
        transform.localScale = targetScale;

        if (targetX == 0f)
        {
            IsFullyOpen = true;
            yield return new WaitForSeconds(_autoCloseDelay);
            CloseDoor();
        }
    }
}
