using UnityEngine;

public class ShowEnding : MonoBehaviour
{
    [SerializeField] private float holdTime;

    private void OnEnable()
    {
        Invoke(nameof(SessionQuizEnd), holdTime);
    }

    private void SessionQuizEnd()
    {
        SessionManager.Instance.QuizDone();
    }
}
