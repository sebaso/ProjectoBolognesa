using System;
using Unity.VisualScripting;
using UnityEngine;

public class Pickeable : MonoBehaviour, IPickeable
{
    [SerializeField]
    private GameObject _pickeableObject;
    [SerializeField]
    private GameObject _playerObject;
    void Start()
    {
        _pickeableObject.SetActive(true);
    }

    public void Pick()
    {
        _pickeableObject.SetActive(false);
        _playerObject.SetActive(true);
    }
}
