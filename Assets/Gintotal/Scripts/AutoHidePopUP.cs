using UnityEngine;

public class AutoHidePopUP : MonoBehaviour
{
    [SerializeField] private float hideTime = 5f;
    [SerializeField] private Outline outline;
        
    private void OnEnable()
    {
        SessionManager.Instance.activeInteraction = false;
        Invoke(nameof(HideUI), hideTime);
    }

    private void HideUI()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        SessionManager.Instance.activeInteraction = true;
        if (SessionManager.Instance.GetSessionName != "Intro") return;
        if (outline != null) outline.enabled = true;
    }
}