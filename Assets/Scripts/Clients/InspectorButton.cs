using UnityEngine;

public class InspectorButton : MonoBehaviour, IInteractable
{
    public enum ButtonType { Accept, Reject }
    public ButtonType buttonType;
    public InspectorSheet inspectorSheet;

    public void Interact()
    {
        if (inspectorSheet == null)
        {
            return;
        }

        if (buttonType == ButtonType.Accept)
        {
            inspectorSheet.AcceptClient();
        }
        else
        {
            inspectorSheet.RejectClient();
        }
    }
}
