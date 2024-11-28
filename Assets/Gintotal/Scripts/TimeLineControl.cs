using UnityEngine;
using UnityEngine.InputSystem; // Input System 사용
using UnityEngine.Playables;  // Timeline 사용

public class TimeLineControl : MonoBehaviour
{
    [SerializeField] private InputActionProperty forwardTrigger; // 오른쪽 컨트롤러 트리거
    [SerializeField] private InputActionProperty backwardTrigger;  // 왼쪽 컨트롤러 트리거
    [SerializeField] private bool customPlay;
    [SerializeField] private GameObject hands;
    
    private PlayableDirector _timeline; // 타임라인 연결
    private const float PlaySpeed = 1f;

    private bool _isReversing;

    private void OnEnable()
    {
        hands.SetActive(false);
        _timeline = GetComponent<PlayableDirector>();
        _timeline.Play();
        _timeline.Pause();
    }

    private void Update()
    {
        if (customPlay)
        {
            if (forwardTrigger.action.IsPressed())
            {
                if (_timeline != null && _timeline.playableAsset != null)
                {
                    if (hands != null) hands.SetActive(true);
                    _timeline.Play();
                    return;
                }
            }
        }
        
        if (_timeline == null || _timeline.playableAsset == null)
        {
            Debug.Log("PlayableDirector가 연결되지 않았습니다.");
            return;
        }

        // 오른쪽 트리거를 누르고 있을 때 정방향 재생
        if (forwardTrigger.action.IsPressed())
        {
            _isReversing = false;
            _timeline.playableGraph.GetRootPlayable(0).SetSpeed(PlaySpeed);
            if (_timeline.state != PlayState.Playing)
            {
                if (hands != null) hands.SetActive(true);
                _timeline.Play();
            }
        }
        // 왼쪽 트리거를 누르고 있을 때 역방향 재생
        else if (backwardTrigger.action.IsPressed())
        {
            _isReversing = true;
            if (_timeline.state == PlayState.Playing)
            {
                if (hands != null) hands.SetActive(true);
                _timeline.Pause();
            } // 역방향 재생에서는 Play 중단
        }
        // 트리거를 놓았을 때 멈춤
        else
        {
            _isReversing = false;
            if (hands != null) hands.SetActive(false);
            _timeline.Pause();
        }

        // 역방향 재생 처리
        if (_isReversing)
        {
            _timeline.time -= Time.deltaTime * PlaySpeed;
            if (_timeline.time < 0) _timeline.time = 0; // 시작 지점에서 멈춤
            _timeline.Evaluate();
        }
    }
}
