using UnityEngine;
using UnityEngine.Playables;

public class Session : MonoBehaviour
{
    public string text = "시작과 관련된 \n텍스트를 적습니다.\n세줄까지 정렬 가능합니다.";
    public bool isAnim = true;
    public bool isPopup = true;
    public bool isCamPose = true;
    public bool isPause = true;
    public bool isInteractable = true;
    public bool isDone = true;

    public void GetDirector()
    {
        if (gameObject.name.Contains("Quiz")) return;
        
        SessionManager.Instance.playableDirector = GetComponent<PlayableDirector>();
    }
}
