using UnityEngine;

public class ShowQuizVisable : MonoBehaviour
{
    private void OnEnable()
    {
        SessionManager.Instance._activeInteraction = false;
        Invoke(nameof(HideUI), 5f);
    }

    private void HideUI()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        SessionManager.Instance._activeInteraction = true;
    }
}