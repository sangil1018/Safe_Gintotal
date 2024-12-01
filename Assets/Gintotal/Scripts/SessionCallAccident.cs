using UnityEngine;

public class SessionCallAccident : MonoBehaviour
{
    [SerializeField] private float delayDoneTime;
    [SerializeField] private AudioSource audioSource;
    
    private void OnEnable()
    {
        if (audioSource != null) delayDoneTime += audioSource.clip.length;
        Invoke(nameof(CallAccident), delayDoneTime);
    }

    private void CallAccident()
    {
        SessionManager.Instance?.SessionCurruptToAccident();
    }
}