using UnityEngine;
using UnityEngine.Playables;

public class Session : MonoBehaviour
{
    [TextArea] public string text = "시작과 관련된\n텍스트를 적습니다.\n세줄까지 정렬 가능합니다.\n추가 텍스트";

    public bool isAnim = true;
    public bool startAnim = true;
    public bool isPopup = true;
    public bool isStartPosition = true;
    public bool isDone = true;
    public bool goToAccident;
    public bool nextSession;

    public bool leftCon;
    public bool rightCon;
    public bool emergency;

    public void GetDirector()
    {
        if (gameObject.name.Contains("Quiz")) return;
        SessionManager.Instance.playableDirector = GetComponent<PlayableDirector>();
    }
}