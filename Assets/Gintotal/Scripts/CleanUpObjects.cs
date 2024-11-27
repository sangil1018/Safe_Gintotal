using UnityEngine;

public class CleanUpObjects : MonoBehaviour
{
    [SerializeField] private int cleanUpCount;
    private int _trashCount;
    private const float DelayTime = 2f;

    public void CleanUp()
    {
        _trashCount++;

        if (_trashCount == cleanUpCount)
        {
            Invoke(nameof(NextSession), DelayTime);
        }
    }

    private void NextSession()
    {
        SessionManager.Instance.SessionDone();
        gameObject.SetActive(false);
    }
}
