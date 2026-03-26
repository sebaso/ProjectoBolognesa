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
        if(HUDController.Instance.IsPanelActive || GameMenuController.Instance.IsPauseActive) return;
        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(_holdRotationOffset);
        transform.localPosition = new Vector3(0f ,0.4f, 0f);
    }

    public void Drop()
    {
        if(HUDController.Instance.IsPanelActive || GameMenuController.Instance.IsPauseActive) return;
        transform.SetParent(null);
        transform.position = _originalPos;
        transform.localRotation = _originalRotation;
    }
}
