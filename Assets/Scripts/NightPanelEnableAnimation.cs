using UnityEngine;

public class NightPanelEnableAnimation : MonoBehaviour
{
    public void OnTryEnableEndNightPanel()
    {
        HUDController.Instance.OnFinishNight();
    }
}
