using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Video;

public class VizUIManager : MonoBehaviour
{
    [SerializeField] public GameObject videoControlUI;
    [SerializeField] public VideoPlayer vp;
    [SerializeField] public Button backBtn;

    private ViveToHome _viveToHome;
    private QuestToHome _questToHome;
    private bool _vShow;

    public void OnMenu(InputAction.CallbackContext context) => _vShow = !_vShow;
    
    private void Awake() => videoControlUI.SetActive(false);
    
    private void Start()
    {
#if UNITY_ANDROID
        _questToHome = GetComponent<QuestToHome>();
        backBtn.onClick.AddListener(_questToHome.BackButtonToHome);
#else
        _viveToHome = GetComponent<ViveToHome>();
        backBtn.onClick.AddListener(_viveToHome.BackButtonToHome);
#endif
        Play360Video();
    }

    private void Update() => videoControlUI.SetActive(_vShow);

    private void Play360Video()
    {
        vp.loopPointReached += EndReached;
        vp.Prepare();
#if UNITY_ANDROID
        _questToHome.CameraToWhite();
#else
        _viveToHome.CameraToWhite();
#endif
        vp.Play();
    }

    private void EndReached(VideoPlayer videoPlayer) => _vShow = true;
    // todo 영상 종료후엔 창 띄우고 5초? 후에 자동으로 홈으로 넘어가게
}
