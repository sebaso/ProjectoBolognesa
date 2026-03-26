using System.Collections.Generic;
using UnityEngine;

public class MonsterMinigame2 : MonoBehaviour
{
    [Header("References for minigame Controller")]
    [SerializeField]
    private RectTransform _detectorParent;
    [SerializeField]
    private RectTransform _maskRevealer;
    [SerializeField]
    private RectTransform _skeletonMonster;
    [SerializeField]
    private RectTransform[] _anomaliesBoxes;
    [SerializeField]
    private RectTransform[] _normalObjBoxes;
    [SerializeField]
    private List<RectTransform> _bodyCheckpoints;
    
    public RectTransform GetDetectorParent()
    {
        return _detectorParent;
    }

    public RectTransform GetMaskRevealer()
    {
        return _maskRevealer;
    }

    public RectTransform GetSkeletonMonster()
    {
        return _skeletonMonster;
    }

    public RectTransform[] GetAnomaliesBoxes()
    {
        return _anomaliesBoxes;
    }

    public RectTransform[] GetNormalObjBoxes()
    {
        return _normalObjBoxes;
    }

    public List<RectTransform> GetBodyCheckpoints()
    {
        return _bodyCheckpoints;
    }
}
