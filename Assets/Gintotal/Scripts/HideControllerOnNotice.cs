using UnityEngine;

public class HideControllerOnNotice : MonoBehaviour
{
    private void OnEnable() => SessionManager.Instance.activeInteraction = true;

    private void OnDisable() => SessionManager.Instance.activeInteraction = false;
}
