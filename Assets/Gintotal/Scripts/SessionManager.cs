using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class SessionManager : MonoBehaviour
{
    private static SessionManager _instance;
    [SerializeField] private GameObject intro;
    [SerializeField] private GameObject[] sessions;
    [SerializeField] private GameObject accident;
    [SerializeField] private GameObject quiz;
    [SerializeField] private GameObject ending;
    [SerializeField] private GameObject popup;
    [SerializeField] private GameObject introPopup;
    [SerializeField] private GameObject startPopup;
    [SerializeField] private GameObject sessionPopup;
    [SerializeField] private GameObject accidentPopup;
    [SerializeField] private GameObject backPopup;
    [SerializeField] private TMP_Text popupText;
    [SerializeField] private GameObject quizMenu;
    [SerializeField] private float startDelayTime = 1f;
    [SerializeField] private float hidePopupTime = 10f;

    public GameObject interaction;
    public Transform playerOrigin;
    public PlayableDirector playableDirector;
    public int currentSessionID;
    public readonly int MAXCount = 3;
    
    private Session _session;
    public Session GetSession => _session;
    public string GETSessionName => _session.gameObject.name;
    private GameObject _uiManager;
    private AudioSource _audioSource;
    private QuestToHome _questToHome;
    private ViveToHome _viveToHome;
    public bool activeInteraction;
    private bool _isPaused;
    private bool _isIntroDone;
    private QuizSet _quizSet;
    private InputActionManager _inputActionManager;

    [SerializeField] private bool quizTester;

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

        popup.SetActive(false);

        _quizSet = quiz.GetComponent<QuizSet>();

        _inputActionManager = GetComponent<InputActionManager>();
        
#if UNITY_ANDROID
        _questToHome = _uiManager.GetComponent<QuestToHome>();
#else
        _viveToHome = _uiManager.GetComponent<ViveToHome>();
#endif
    }

    public InputActionManager GetInputActionManager()
    {
        return _inputActionManager;
    }

    public static SessionManager Instance => null == _instance ? null : _instance;

    private void Start()
    {
        switch (quizTester)
        {
            case true:
                ShowQuiz();
                break;
            default:
                FindChildSessions();
                ShowIntro();
                break;
        }
    }

    private void Update()
    {
        interaction.SetActive(activeInteraction && !introPopup.activeSelf && !startPopup.activeSelf && !sessionPopup.activeSelf && !accidentPopup.activeSelf && !backPopup.activeSelf);
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

    public void ShowBackUI(bool show) => backPopup.SetActive(show);

    private void ShowIntro()
    {
        intro.SetActive(true);
        _session = intro.GetComponent<Session>();
        _session.SetStartingPosition();
        _session.RefreshControllers();
        
        SetPopUp("Intro");
        ProcessingSession();
    }

    public void IntroDone()
    {
        FadeBlack();
        intro.SetActive(false);
        ShowSession();
    }

    private void ShowSession()
    {
        sessions[currentSessionID].SetActive(true);
        _session = sessions[currentSessionID].GetComponent<Session>();
        _session.SetStartingPosition();
        
        SetPopUp();
        ProcessingSession();
    }

    public void SessionDoneDelay(float timeSpan) => Invoke(nameof(SessionDone), timeSpan);

    public void SessionDone()
    {
        FadeBlack();
        
        sessions[currentSessionID].SetActive(false);
        currentSessionID += 1;
        
        if (_session.isDone) ShowAccident();
        else ShowSession();
    }

    private void ShowAccident()
    {
        hidePopupTime = 10f;
        accident.SetActive(true);
        _session = accident.GetComponent<Session>();
        _session.SetStartingPosition();
        
        SetPopUp("Accident");
        ProcessingSession();
    }

    public void AccidentDone()
    {
        FadeBlack();
        accident.SetActive(false);
        Invoke(nameof(ShowQuiz), startDelayTime);
        // ShowQuiz();
    }

    private void ShowQuiz()
    {
        hidePopupTime = 5f;
        quiz.SetActive(true);
        _session = quiz.GetComponent<Session>();
        _session.SetStartingPosition();
        
        if (!_session.isDone)
        {
            quizMenu.SetActive(true);
            _quizSet.InitialQuiz();
        }

        SetPopUp("Quiz");
        ProcessingSession();
    }
    
    public void QuizDone()
    {
        ShowEnding();
        
        quiz.SetActive(false);
        quizMenu.SetActive(false);
    }
    
    private void ShowEnding()
    {
        ending.SetActive(true);
        _session = ending.GetComponent<Session>();
        _session.SetStartingPosition();
        
        ProcessingSession();
    }

    private void ProcessingSession()
    {
        activeInteraction = !(_session.isAnim || _session.isPopup);
        
        popupText.text = _session.text;
        
        if (_session.isAnim)
        {
            playableDirector = _session.GetComponent<PlayableDirector>();
            // _session.GetDirector();
            // playableDirector.stopped += OnPlayableDirectorStopped;
        }
        _audioSource = _session.GetComponent<AudioSource>();

        FadeWhite();
        
        if (_session.isPopup)
        {
            Invoke(nameof(DelayPopUp), 1f);
            if (_audioSource.clip != null) hidePopupTime = _audioSource.clip.length;
            
            Invoke(nameof(HidePopUp), hidePopupTime + 1f);
        }
        else
        {
            if (_session.isAnim && _session.startAnim)
            {
                Invoke(nameof(PlayAnimation), startDelayTime);
            }
            else
            {
                if (!_session.isDone) return;
                switch (_session.gameObject.name.ToLower())
                {
                    case "intro":
                        IntroDone();
                        break;
                    case "accident":
                        AccidentDone();
                        break;
                    case "quiz":
                        QuizDone();
                        break;
                    case "ending":
                        StartCoroutine(EndingProcess());
                        break;
                    default:
                        SessionDone();
                        break;
                }
            }
        }
    }

    private void DelayPopUp() => popup.SetActive(true);

    private void SetPopUp(string sessionName = "Session")
    {
        switch (sessionName.ToLower())
        {
            case "intro":
                introPopup.SetActive(_session.isPopup);
                sessionPopup.SetActive(false);
                accidentPopup.SetActive(false);
                popup = introPopup;
                break;
            case "accident":
                introPopup.SetActive(false);
                sessionPopup.SetActive(false);
                accidentPopup.SetActive(_session.isPopup);
                popup = accidentPopup;
                activeInteraction = true;
                break;
            default:
                // 인터렉션 활성화
                introPopup.SetActive(false);
                // if (sessionName.Contains("session")) popupText.text = _session.text;
                sessionPopup.SetActive(_session.isPopup);
                accidentPopup.SetActive(false);
                popup = sessionPopup;
                activeInteraction = true;
                break;
        }
    }

    private void HidePopUp()
    {
        popup.SetActive(false);
        
        if (_session.isAnim && _session.startAnim)
        {
            Invoke(nameof(PlayAnimation), startDelayTime);
            activeInteraction = true;
        }
        else
        {
            switch (_session.gameObject.name.ToLower())
            {
                case "intro":
                    startPopup.SetActive(true);
                    break;
                case "accident":
                    // Invoke(nameof(ShowQuiz), startDelayTime);
                    AccidentDone();
                    break;
                case "ending":
                    StartCoroutine(EndingProcess());
                    break;
                default:
                    // 인터렉션 활성화
                    activeInteraction = true;
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
        activeInteraction = true;
        
        playableDirector.stopped -= OnPlayableDirectorStopped;
        
        // 체험이 있으면 애니 종료후 대기함
        // if (_session.isExp) return;
        
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

    private IEnumerator EndingProcess()
    {
        yield return new WaitForSeconds(2f);
        
        ShowBackUI(true);
        
        yield return new WaitForSeconds(3f);

        FadeBlack();
        
        yield return new WaitForSeconds(2f);
        
#if UNITY_ANDROID
        _questToHome.BackButtonToHome();
#else
        _viveToHome.BackButtonToHome();
#endif
    }
    
    // [SerializeField] private Transform moveTarget;
    private const float Duration = 6f;
    private string _doneSessionName;
    
    public void PlayerMove(string mDoneSessionName)
    {
        var moveTarget = _session.transform.position;
        _doneSessionName = mDoneSessionName;
        playerOrigin.DOMove(moveTarget, Duration, true);
        
        Invoke(nameof(ExcuteSessionDone), Duration + 1f);
    }

    private void ExcuteSessionDone()
    {
        switch (_doneSessionName.ToLower())
        {
            case "intro":
                IntroDone();
                return;
            case "session":
                SessionDone();
                return;
            case "accident":
                AccidentDone();
                return;
            case "quiz":
                QuizDone();
                return;
            default:
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

    public void AccidentHappen()
    {
#if UNITY_ANDROID
        _questToHome.FaderSequence();
#else
        _viveToHome.FaderSequence();
#endif
    }
}
