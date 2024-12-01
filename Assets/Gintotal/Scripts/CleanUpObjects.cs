using UnityEngine;

public class CleanUpObjects : MonoBehaviour
{
    [SerializeField] private int cleanUpCount;
    private int _trashCount;
    [SerializeField] private float delayTime = 2f;

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
        gameObject.SetActive(false);
    }
}
