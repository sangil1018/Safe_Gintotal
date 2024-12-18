using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class SessionManager : MonoBehaviour
{
    // [SerializeField] private Transform moveTarget;
    private const float Duration = 6f;
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
    public GameObject leftController;
    public GameObject rightController;
    public Transform playerOrigin;
    public PlayableDirector playableDirector;
    public int currentSessionID;
    public bool activeInteraction;

    [SerializeField] private bool quizTester;
    public readonly int MAXCount = 3;
    private AudioSource _audioSource;
    private string _doneSessionName;
    private InputActionManager _inputActionManager;
    private bool _isPlaying;
    private QuestToHome _questToHome;

    // private bool _isIntroDone;
    private QuizSet _quizSet;

    private GameObject _uiManager;
    private ViveToHome _viveToHome;
    public Session GetSession { get; private set; }

    public string GETSessionName => GetSession.gameObject.name;

    public static SessionManager Instance => null == _instance ? null : _instance;

    private void Awake()
    {
        _instance = this;
        // if (null == _instance)
        // {
        //     _instance = this;
        //     DontDestroyOnLoad((gameObject));
        // }
        // else
        // {
        //     Destroy(gameObject);
        // }

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
        _isPlaying = playableDirector != null ? playableDirector.state == PlayState.Playing : false;
        interaction.SetActive(!activeInteraction && !popup.activeSelf && !startPopup.activeSelf &&
                              !backPopup.activeSelf && !_isPlaying);
        leftController.SetActive(!activeInteraction && !GetSession.leftCon && !popup.activeSelf &&
                                 !startPopup.activeSelf &&
                                 !backPopup.activeSelf && !_isPlaying);
        rightController.SetActive(!activeInteraction && !GetSession.rightCon && !popup.activeSelf &&
                                  !startPopup.activeSelf &&
                                  !backPopup.activeSelf && !_isPlaying);
    }

    public InputActionManager GetInputActionManager()
    {
        return _inputActionManager;
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

    public void ShowBackUI(bool show)
    {
        backPopup.SetActive(show);
    }

    private void ShowIntro()
    {
        intro.SetActive(true);
        GetSession = intro.GetComponent<Session>();
        GetSession.SetStartingPosition();
        // _session.RefreshControllers();

        SetPopUp("Intro");
        ProcessingSession();
    }

    public void IntroDone()
    {
        FadeBlack();
        intro.SetActive(false);
        ShowSession();
    }

    public void ShowSession()
    {
        sessions[currentSessionID].SetActive(true);
        GetSession = sessions[currentSessionID].GetComponent<Session>();
        GetSession.SetStartingPosition();

        SetPopUp();
        ProcessingSession();
    }

    public void SessionDoneDelay(float timeSpan)
    {
        Invoke(nameof(SessionDone), timeSpan);
    }

    public void SessionDone()
    {
        FadeBlack();

        sessions[currentSessionID].SetActive(false);
        currentSessionID += 1;

        if (GetSession.isDone) ShowAccident();
        else ShowSession();
    }

    public void SessionCurruptToAccident()
    {
        FadeBlack();

        sessions[currentSessionID].SetActive(false);
        currentSessionID += 1;

        ShowAccident();
    }

    public void ShowAccident()
    {
        hidePopupTime = 10f;
        accident.SetActive(true);
        GetSession = accident.GetComponent<Session>();
        GetSession.SetStartingPosition();

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

    public void ShowQuiz()
    {
        hidePopupTime = 5f;
        quiz.SetActive(true);
        GetSession = quiz.GetComponent<Session>();
        GetSession.SetStartingPosition();

        if (!GetSession.isDone)
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
        GetSession = ending.GetComponent<Session>();
        GetSession.SetStartingPosition();

        ProcessingSession();
    }

    private void ProcessingSession()
    {
        popupText.text = GetSession.text;

        if (GetSession.isAnim) playableDirector = GetSession.GetComponent<PlayableDirector>();

        FadeWhite();

        if (GetSession.isPopup)
        {
            _audioSource = GetSession.GetComponent<AudioSource>();

            Invoke(nameof(DelayPopUp), 1f);
            if (_audioSource.clip != null) hidePopupTime = _audioSource.clip.length + 2f;

            Invoke(nameof(HidePopUp), hidePopupTime);
        }
        else
        {
            if (GetSession.isAnim && GetSession.startAnim)
            {
                Invoke(nameof(PlayAnimation), startDelayTime);
            }
            else
            {
                if (GetSession.goToAccident) SessionCurruptToAccident();
                if (!GetSession.isDone) return;
                switch (GetSession.gameObject.name.ToLower())
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

    private void DelayPopUp()
    {
        popup.SetActive(true);
        if (_audioSource.clip != null) _audioSource.Play();
    }

    private void SetPopUp(string sessionName = "Session")
    {
        switch (sessionName.ToLower())
        {
            case "intro":
                introPopup.SetActive(GetSession.isPopup);
                sessionPopup.SetActive(false);
                accidentPopup.SetActive(false);
                popup = introPopup;
                break;
            case "accident":
                introPopup.SetActive(false);
                sessionPopup.SetActive(false);
                accidentPopup.SetActive(GetSession.isPopup);
                popup = accidentPopup;
                // activeInteraction = true;
                break;
            default:
                // 인터렉션 활성화
                introPopup.SetActive(false);
                // if (sessionName.Contains("session")) popupText.text = _session.text;
                sessionPopup.SetActive(GetSession.isPopup);
                accidentPopup.SetActive(false);
                popup = sessionPopup;
                // activeInteraction = true;
                break;
        }

        // Invoke(nameof(PlayAudioPopUp), 2f);
    }

    private void PlayAudioPopUp()
    {
        _audioSource.Play();
    }

    private void HidePopUp()
    {
        popup.SetActive(false);

        if (GetSession.isAnim && GetSession.startAnim)
        {
            Invoke(nameof(PlayAnimation), startDelayTime);
            // activeInteraction = true;
        }
        else
        {
            if (GetSession.goToAccident) SessionCurruptToAccident();
            else
                switch (GetSession.gameObject.name.ToLower())
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
                        if (GetSession.nextSession) SessionDone();
                        break;
                }
        }
    }

    public void PlayAnimation()
    {
        playableDirector.Play();
    }

    public void StopAnimation()
    {
        playableDirector.Stop();
    }

    public void PauseAnimation()
    {
        playableDirector.Pause();
    }

    public void ResumeAnimation()
    {
        playableDirector.Resume();
    }

    private IEnumerator EndingProcess()
    {
#if UNITY_ANDROID

#else
        yield return new WaitForSeconds(2f);
        
        ShowBackUI(true);
#endif
        yield return new WaitForSeconds(3f);

        FadeBlack();

        yield return new WaitForSeconds(2f);

// #if UNITY_ANDROID
//         _questToHome.BackButtonToHome();
// #else
//         _viveToHome.BackButtonToHome();
// #endif
    }

    public void PlayerMove(string mDoneSessionName)
    {
        var moveTarget = GetSession.transform.position;
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