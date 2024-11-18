using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class SessionManager : MonoBehaviour
{
    private static SessionManager _instance;
    [SerializeField] public GameObject intro;
    [SerializeField] public GameObject[] sessions;
    [SerializeField] public GameObject accident;
    [SerializeField] public GameObject quiz;
    [SerializeField] public GameObject ending;
    [SerializeField] public GameObject interaction;
    [SerializeField] public TMP_Text noticeText;
    [SerializeField] public TMP_Text popupText;
    [SerializeField] public GameObject quizMenu;

    [SerializeField] public float startDelayTime = 2f;
    [SerializeField] public float hidePopupTime = 5f;

    public Transform playerOrigin;
    public PlayableDirector playableDirector;
    public int currentSessionID;
    public int answerCorrect;
    public readonly int MAXCount = 3;
    
    private Session _session;
    private GameObject _uiManager;
    private GameObject _notice;
    private GameObject _popup;
    private AudioSource _audioSource;
    private QuestToHome _questToHome;
    private ViveToHome _viveToHome;
    private bool _activeInteraction;
    private bool _isPaused;
    
    private void Awake()
    {
        if (null == _instance)
        {
            _instance = this;
            DontDestroyOnLoad((gameObject));
        }
        else
        {
            Destroy(gameObject);
        }
        
        _uiManager = GameObject.Find("UI Manager");

        _notice = noticeText.transform.parent.parent.parent.parent.gameObject;
        _notice.SetActive(false);
        _popup = popupText.transform.parent.parent.parent.parent.gameObject;
        _popup.SetActive(false);
        
#if UNITY_ANDROID
        _questToHome = _uiManager.GetComponent<QuestToHome>();
#else
        _viveToHome = _uiManager.GetComponent<ViveToHome>();
#endif
    }
    
    public static SessionManager Instance => null == _instance ? null : _instance;

    private void Start()
    {
        FindChildSessions();
        ShowIntro();
    }

    private void Update()
    {
        interaction.SetActive(_activeInteraction);
    }

    private void FindChildSessions()
    {
        var objs = new List<GameObject>();
        for (var i = 0; i < transform.childCount; i++)
        {
            var obj = transform.GetChild(i).gameObject;
            if (obj.name.ToLower().Contains("session")) objs.Add(obj);
            obj.SetActive(false);
        }

        sessions = objs.ToArray();
        currentSessionID = 0;
    }

    private void ShowIntro()
    {
        intro.SetActive(true);
        _session = intro.GetComponent<Session>();

        ProcessingSession();
    }

    public void IntroDone()
    {
        FadeBlack();
        ShowSession();
        
        intro.SetActive(false);
    }

    private void ShowSession()
    {
        sessions[currentSessionID].SetActive(true);
        _session = sessions[currentSessionID].GetComponent<Session>();
        
        ProcessingSession();
    }

    public void SessionDone()
    {
        FadeBlack();
        currentSessionID++;
        
        if (_session.isDone) ShowAccident();
        else ShowSession();
        
        sessions[currentSessionID-1].SetActive(false);
    }

    private void ShowAccident()
    {
        hidePopupTime = 10f;
        accident.SetActive(true);
        _session = accident.GetComponent<Session>();
        
        ProcessingSession();
    }

    public void AccidentDone()
    {
        FadeBlack();
        ShowQuiz();
        
        accident.SetActive(false);
    }

    private void ShowQuiz()
    {
        hidePopupTime = 5f;
        quiz.SetActive(true);
        _session = quiz.GetComponent<Session>();
        quizMenu.SetActive(true);
        
        ProcessingSession();
    }
    
    public void QuizDone()
    {
        FadeBlack();
        ShowEnding();
        // todo: 엔딩 관련 무언가 하던지 아니면 돌아가기 버튼 보이기
        // _uiManager.GetComponent<ExpUIManager>().ShowBackButton();
        
        quiz.SetActive(false);
    }
    
    private void ShowEnding()
    {
        ending.SetActive(true);
        _session = ending.GetComponent<Session>();
        
        ProcessingSession();
    }

    private void ProcessingSession()
    {
        _activeInteraction = !(_session.isAnim || _session.isPopup);
        _session.SetStartingPosition();
        noticeText.text = _session.text;
        if (_session.isAnim)
        {
            _session.GetDirector();
            playableDirector.stopped += OnPlayableDirectorStopped;
        }
        _audioSource = _session.GetComponent<AudioSource>();

        FadeWhite();
        
        if (_session.isPopup)
        {
            _notice.SetActive(true);
            if (_audioSource.clip != null) hidePopupTime = _audioSource.clip.length;
            
            Invoke(nameof(HidePopUp), hidePopupTime);
        }
        else
        {
            if (_session.isAnim && _session.startAnim)
            {
                Invoke(nameof(PlayAnimation), startDelayTime);
            }
        }
    }

    private void HidePopUp()
    {
        _notice.SetActive(false);
        
        if (_session.isAnim && _session.startAnim)
        {
            Invoke(nameof(PlayAnimation), startDelayTime);
        }
        else
        {
            switch (_session.gameObject.name)
            {
                case "Intro":
                    _popup.SetActive(true);
                    break;
                case "Accident":
                    Invoke(nameof(ShowQuiz), startDelayTime);
                    break;
                default:
                    // 인터렉션 활성화
                    _activeInteraction = true;
                    break;
            }
        }
    }

    public void PlayAnimation() => playableDirector.Play();
    public void PauseAnimation() => playableDirector.Pause();
    public void ResumeAnimation() => playableDirector.Resume();

    private void OnPlayableDirectorStopped(PlayableDirector aDirector)
    {
        if (playableDirector == aDirector)
            Debug.Log("PlayableDirector named " + aDirector.name + " is now stopped.");
        
        // 인터렉션 활성화
        _activeInteraction = true;
        
        playableDirector.stopped -= OnPlayableDirectorStopped;
        
        // 체험이 있으면 애니 종료후 대기함
        if (_session.isExp) return;
        
        switch (_session.gameObject.name)
        {
            case "Intro":
                IntroDone();
                return;
            case "Accident":
                AccidentDone();
                return;
            case "Quiz":
                QuizDone();
                return;
            default:
                SessionDone();
                return;
        }
    }

    public void FadeWhite()
    {
#if UNITY_ANDROID
        _questToHome.CameraToWhite();
#else
        _viveToHome.CameraToWhite();
#endif
    }
    
    public void FadeBlack()
    {
#if UNITY_ANDROID
        _questToHome.CameraToBlack();
#else
        _viveToHome.CameraToBlack();
#endif
    }
}
