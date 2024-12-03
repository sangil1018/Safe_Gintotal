using System;
using UnityEngine;

public class EnableInteractables : MonoBehaviour
{
    [SerializeField] private string objectsShowSession;
    [SerializeField] private GameObject[] objects;
    [SerializeField] private GameObject[] hides;
    [SerializeField] private Outline[] outlines;

    private void OnDisable()
    {
        if (!string.Equals(SessionManager.Instance.GETSessionName, objectsShowSession,
            StringComparison.CurrentCultureIgnoreCase)) return;

        if (objects.Length > 0)
        {
            foreach (var obj in objects)
            {
                obj.SetActive(true);
            }
        }
        
        if (hides.Length > 0)
        {
            foreach (var obj in hides)
            {
                obj.SetActive(false);
            }
        }
        
        if (outlines.Length > 0)
        {
            foreach (var outline in outlines)
            {
                outline.enabled = true;
            }

        }
        
        enabled = false;
    }
}