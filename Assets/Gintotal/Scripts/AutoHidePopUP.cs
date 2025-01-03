using System;
using UnityEngine;

public class AutoHidePopUP : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private float hideTime = 5f;
    [SerializeField] private Outline[] outlines;
    [SerializeField] private bool sessionDone;
    
    private void OnEnable()
    {
        if (_audioSource != null)
        {
            if (_audioSource.clip != null)
            {
                _audioSource.Play();
                hideTime = _audioSource.clip.length;
            }
        }
        // SessionManager.Instance.activeInteraction = false;
        Invoke(nameof(HideUI), hideTime + 1f);
    }

    private void HideUI()
    {
        if (sessionDone)
        {
            // SessionManager.Instance.activeInteraction = true;
            if (SessionManager.Instance.GETSessionName == "Intro")
            {
                SessionManager.Instance.IntroDone();
            }
            else
            {
                SessionManager.Instance.SessionDone();
            }
        }

        if (outlines.Length > 0)
        {
            foreach (var outline in outlines)
            {
                outline.enabled = true;
            }
        }
        
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        // SessionManager.Instance.GetSession.RefreshInputActions();
    }
}