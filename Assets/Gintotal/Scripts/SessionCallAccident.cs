using UnityEngine;

public class SessionCallAccident : MonoBehaviour
{
    private void OnEnable()
    {
        SessionManager.Instance?.SessionCurruptToAccident();
    }
}