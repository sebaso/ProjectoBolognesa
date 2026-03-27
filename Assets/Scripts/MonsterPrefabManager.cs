using UnityEngine;

public class MonsterPrefabManager : MonoBehaviour
{
    [Header("Minigame Prefabs")]
    [SerializeField]
    private GameObject _minigame2Prefab;
    [SerializeField]
    private GameObject _minigame3Prefab;

    public GameObject GetMinigame2Prefab()
    {
        return _minigame2Prefab;
    }
    
    public GameObject GetMinigame3Prefab()
    {
        return _minigame3Prefab;
    }
}
