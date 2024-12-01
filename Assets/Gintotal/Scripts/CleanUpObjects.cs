using System;
using UnityEngine;

public class CleanUpObjects : MonoBehaviour
{
    [SerializeField] private int cleanUpCount;
    private int _trashCount;
    [SerializeField] private float delayTime = 2f;

    private void Awake()
    {
        _trashCount = 0;
    }

    public void CleanUp()
    {
        _trashCount++;

        if (_trashCount == cleanUpCount)
        {
            Invoke(nameof(NextSession), delayTime);
        }
    }

    private void NextSession()
    {
        SessionManager.Instance.SessionDone();
        // gameObject.SetActive(false);
    }
}
