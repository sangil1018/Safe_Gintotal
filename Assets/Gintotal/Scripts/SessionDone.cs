using UnityEngine;

public class SessionDone : MonoBehaviour
{
    [SerializeField] private float delayDoneTime;
    private void OnDisable()
    {
        SessionManager.Instance.SessionDoneDelay(delayDoneTime);
    }
}
