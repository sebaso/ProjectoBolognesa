using System;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [SerializeField]
    private Image _crossHairOpen;
    [SerializeField]
    private Image _crossHairClosed;
    [SerializeField]
    private PlayerController _player;
    void Start()
    {
        _crossHairOpen.gameObject.SetActive(true);
    }
    void OnEnable()
    {
        _player.OnHoldTool += UpdateCrossHair;
    }

    void OnDisable()
    {
        _player.OnHoldTool -= UpdateCrossHair;
    }
    private void UpdateCrossHair(bool grabTool)
    {
        if (grabTool)
        {
            _crossHairOpen.gameObject.SetActive(false);
            _crossHairClosed.gameObject.SetActive(true);
        }
        else
        {
            _crossHairClosed.gameObject.SetActive(false);
            _crossHairOpen.gameObject.SetActive(true);
        }
    }
}
