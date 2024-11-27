using System;
using UnityEngine;

public class EnableInteractableObjects : MonoBehaviour
{
    [SerializeField] private string objectsShowSession;
    [SerializeField] private GameObject[] objects;

    private void OnDisable()
    {
        // if (objectsShowSession == null)
        // {
        //     objectsShowSession = SessionManager.Instance.GETSessionName;
        // }
        
        if (!string.Equals(SessionManager.Instance.GETSessionName, objectsShowSession,
            StringComparison.CurrentCultureIgnoreCase)) return;

        if (objects.Length <= 0) return;
        foreach (var obj in objects)
        {
            obj.SetActive(true);
        }

        enabled = false;
    }
}
