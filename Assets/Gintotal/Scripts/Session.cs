using UnityEngine;
using UnityEngine.Playables;

public class Session : MonoBehaviour
{
    public string text = "시작과 관련된 \n텍스트를 적습니다.\n세줄까지 정렬 가능합니다.";
    public bool isAnim = true;
    public bool startAnim = true;
    public bool isPopup = true;
    public bool isStartPosition = true;
    public bool isExp = true;
    public bool isDone = true;
    public bool notice;

    public void SetStartingPosition()
    {
        if (!isStartPosition) return;
        var transform1 = transform;
        SessionManager.Instance.playerOrigin.SetPositionAndRotation(transform1.position, transform1.rotation);
    }

    public void GetDirector()
    {
        if (gameObject.name.Contains("Quiz")) return;
        
        SessionManager.Instance.playableDirector = GetComponent<PlayableDirector>();
    }
}
