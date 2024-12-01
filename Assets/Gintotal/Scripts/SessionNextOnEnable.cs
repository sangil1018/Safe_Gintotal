using UnityEngine;

public class SessionNextOnEnable : MonoBehaviour
{
    [SerializeField] private float delayDoneTime;
    private void OnEnable()
    {
        SessionManager.Instance?.SessionDoneDelay(delayDoneTime);
    }
}
