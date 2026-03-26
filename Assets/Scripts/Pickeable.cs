using System;
using UnityEngine;

public class Pickeable : MonoBehaviour, IPickeable
{
    private Vector3 _originalPos;
    private Quaternion _originalRotation;
    [SerializeField] 
    private Vector3 _holdRotationOffset;
    public void Start()
    {
        _originalPos = transform.position;
        _originalRotation = transform.localRotation;
    }
    public void Pick(Transform holdPoint)
    {
        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(_holdRotationOffset);
    }

    public void Drop()
    {
        transform.SetParent(null);
        transform.position = _originalPos;
        transform.localRotation = _originalRotation;
    }
}
