using UnityEngine;

public class MonsterMinigame3 : MonoBehaviour
{
    [Header("References for minigameController")]
    [SerializeField]
    private RectTransform _backgroundArea;
    [SerializeField]
    private RectTransform _pupil;

    public RectTransform GetBackGroundArea()
    {
        return _backgroundArea;
    }

    public RectTransform GetPupil()
    {
        return _pupil;
    }
}
