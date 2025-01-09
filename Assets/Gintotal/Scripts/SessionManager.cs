using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class SessionManager : MonoBehaviour
{
    private const float Duration = 6f;
    private static SessionManager _instance;
    [SerializeField] private GameObject intro;
    [SerializeField] private GameObject[] sessions;
    [SerializeField] private GameObject accident;
    [SerializeField] private GameObject quiz;
    [SerializeField] private GameObject ending;
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
    private PlayerController _playerController;

    private QuestToHome _questToHome;
    private QuizSet _quizSet;

    private GameObject _uiManager;
    private ViveToHome _viveToHome;
    private GameObject accidentPopup;
    private GameObject backPopup;
    private GameObject introPopup;
    private GameObject popup;
    private TMP_Text popupText;
    private GameObject quizMenu;
    private GameObject sessionPopup;
    private GameObject startPopup;
    public Session GetSession { get; private set; }

    public string GETSessionName => GetSession.gameObject.name;

    public static SessionManager Instance => null == _instance ? null : _instance;

    public bool teleportEndSession { private get; set; }

    private void Awake()
    {
        _instance = this;

        var uiGroup = GameObject.Find("UI Group");
        introPopup = uiGroup.transform.GetChild(0).gameObject;
        startPopup = uiGroup.transform.GetChild(1).gameObject;
        sessionPopup = uiGroup.transform.GetChild(2).gameObject;
        accidentPopup = uiGroup.transform.GetChild(3).gameObject;
        backPopup = uiGroup.transform.GetChild(4).gameObject;
        quizMenu = uiGroup.transform.GetChild(5).gameObject;
        popup = introPopup;
        popupText = sessionPopup.GetComponentInChildren<TMP_Text>();
        _uiManager = GameObject.Find("UI Manager");
        popup.SetActive(false);

        _quizSet = quiz.GetComponent<QuizSet>();
        _inputActionManager = GetComponent<InputActionManager>();
        _questToHome = _uiManager.GetComponent<QuestToHome>();
        // _questToHome.CameraBK(); // 화면 검게 만들어주기
        _playerController = playerOrigin.GetComponent<PlayerController>();
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
                break;
        }
    }

    private void Update()
    {
        _isPlaying = playableDirector != null && playableDirector.state == PlayState.Playing;
        if (GetSession.emergency) _isPlaying = false;
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

        ShowIntro();
    }

    private void ShowIntro()
    {
        intro.SetActive(true);
        GetSession = intro.GetComponent<Session>();

        SetPopUp("Intro");
        ProcessingSession();
    }

    public void IntroDone()
    {
        FadeBlack();
        Invoke(nameof(IntroDoneWait), 1);
    }

    private void IntroDoneWait()
    {
        intro.SetActive(false);
        ShowSession();
    }

    public void ShowSession()
    {
        sessions[currentSessionID].SetActive(true);
        GetSession = sessions[currentSessionID].GetComponent<Session>();

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
        Invoke(nameof(SessionDoneWait), 1);
    }

    private void SessionDoneWait()
    {
        sessions[currentSessionID].SetActive(false);
        currentSessionID += 1;

        if (GetSession.isDone) ShowAccident();
        else ShowSession();
    }

    public void SessionCurruptToAccident()
    {
        FadeBlack();
        Invoke(nameof(SessionCurruptWait), 1);
    }

    private void SessionCurruptWait()
    {
        sessions[currentSessionID].SetActive(false);
        currentSessionID += 1;
        ShowAccident();
    }

    public void ShowAccident()
    {
        hidePopupTime = 10f;
        accident.SetActive(true);
        GetSession = accident.GetComponent<Session>();

        SetPopUp("Accident");
        ProcessingSession();
    }

    public void AccidentDone()
    {
        FadeBlack();
        Invoke(nameof(AccidentDoneWait), 1);
        // ShowQuiz();
    }

    private void AccidentDoneWait()
    {
        accident.SetActive(false);
        Invoke(nameof(ShowQuiz), startDelayTime);
    }

    public void ShowQuiz()
    {
        hidePopupTime = 5f;
        quiz.SetActive(true);
        GetSession = quiz.GetComponent<Session>();

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
        FadeBlack();
        Invoke(nameof(ShowEnding), 1);
    }

    private void ShowEnding()
    {
        ending.SetActive(true);
        GetSession = ending.GetComponent<Session>();

        ProcessingSession();

        quiz.SetActive(false);
        quizMenu.SetActive(false);
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

    public void ReCenterPlayer()
    {
        if (!GetSession.isStartPosition) return;
        _playerController.resetTransform = GetSession.transform;
        _playerController.ResetPosition();
    }

    public void ReCenterPlayer(Transform target)
    {
        StartCoroutine(nameof(RecenterSequence), target);
    }

    private IEnumerator RecenterSequence(Transform target)
    {
        _playerController.resetTransform = target;

        FadeBlack();
        yield return new WaitForSeconds(1);

        _playerController.ResetPosition();

        yield return null;

        FadeWhite();
        yield return new WaitForSeconds(1);
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
                break;
            default:
                // 인터렉션 활성화
                introPopup.SetActive(false);
                sessionPopup.SetActive(GetSession.isPopup);
                accidentPopup.SetActive(false);
                popup = sessionPopup;
                break;
        }
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
        // yield return new WaitForSeconds(0.5f);

        backPopup.SetActive(true);
        yield return new WaitForSeconds(3.5f);

        FadeBlack();

        yield return new WaitForSeconds(0.5f);

        _questToHome.BackButtonToHome();
    }

    public void GoToHome()
    {
        _questToHome.BackButtonToHome();
    }

    public void PlayerMove(string mDoneSessionName)
    {
        var moveTarget = GetSession.transform.position;
        _doneSessionName = mDoneSessionName;
        playerOrigin.DOMove(moveTarget, Duration, true);

        Invoke(nameof(ExcuteSessionDone), Duration + 1f);
    }

    public void CameraMove(Transform target)
    {
        const float duration = 2f;
        playerOrigin.DOMove(target.position, duration, true);
    }

    public void Teleport(Transform target)
    {
        const float duration = 3.5f;
        playerOrigin.DOMove(target.position, duration, true);

        if (!teleportEndSession) return;
        Invoke(nameof(SessionDone), duration + 0.1f);
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
        _questToHome.CameraToWhite();
    }

    public void FadeBlack()
    {
        _questToHome.CameraToBlack();
    }

    public void AccidentHappen()
    {
        _questToHome.FaderSequence();
    }
}