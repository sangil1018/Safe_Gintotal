using UnityEngine;

public class AutoHidePopUP : MonoBehaviour
{
    [SerializeField] private float hideTime = 5f;
    [SerializeField] private Outline[] outlines;
        
    private void OnEnable()
    {
        SessionManager.Instance.activeInteraction = false;
        Invoke(nameof(HideUI), hideTime);
    }

    private void HideUI()
    {
        SessionManager.Instance.activeInteraction = true;
        if (SessionManager.Instance.getSessionName == "Intro") SessionManager.Instance.IntroDone();
        if (outlines.Length > 0)
        {
            foreach (var outline in outlines)
            {
                outline.enabled = true;
            }
        }
        
        gameObject.SetActive(false);
    }
}