using UnityEngine;

public class SessionNextOnEnable : MonoBehaviour
{
    [SerializeField] private float delayDoneTime;
    [SerializeField] private AudioSource audioSource;
    
    private void OnEnable()
    {
        if (audioSource != null) delayDoneTime += audioSource.clip.length;
        SessionManager.Instance?.SessionDoneDelay(delayDoneTime);
    }
}
