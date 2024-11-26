using System;
using UnityEngine;

public class EnableInteractableOutlines : MonoBehaviour
{
    [SerializeField] private string objectsShowSession;
    [SerializeField] private Outline[] outlines;

    private void OnDisable()
    {
        objectsShowSession ??= SessionManager.Instance.getSessionName;
        
        if (!string.Equals(SessionManager.Instance.getSessionName, objectsShowSession,
            StringComparison.CurrentCultureIgnoreCase)) return;

        if (outlines.Length <= 0) return;
        foreach (var outline in outlines)
        {
            outline.enabled = true;
        }

        enabled = false;
    }
}
