using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

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
    [SerializeField] private TMP_Text popupText;
    [SerializeField] private GameObject quizMenu;
    [SerializeField] private float startDelayTime = 2f;
    [SerializeField] private float hidePopupTime = 10f;

    public GameObject interaction;
    public Transform playerOrigin;
    public PlayableDirector playableDirector;
    public int currentSessionID;
    public readonly int MAXCount = 3;
    
    private Session _session;
    public string GetSessionName => _session.gameObject.name;
    private GameObject _uiManager;
    private AudioSource _audioSource;
    private QuestToHome _questToHome;
    private ViveToHome _viveToHome;
    public bool activeInteraction;
    private bool _isPaused;
    private bool _isIntroDone;
    private QuizSet _quizSet;

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
        interaction.SetActive(activeInteraction);
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
        SetPopUp();
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
        SetPopUp("Accident");
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
        _quizSet.InitialQuiz();
        
        SetPopUp("Quiz");
        ProcessingSession();
    }
    
    public void QuizDone()
    {
        ShowEnding();
        
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
        activeInteraction = !(_session.isAnim || _session.isPopup);
        _session.SetStartingPosition();
        popupText.text = _session.text;
        
        if (_session.isAnim)
        {
            _session.GetDirector();
            // playableDirector.stopped += OnPlayableDirectorStopped;
        }
        _audioSource = _session.GetComponent<AudioSource>();

        FadeWhite();
        
        if (_session.isPopup)
        {
            Invoke(nameof(DelayPopUp), 2f);
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

    private void DelayPopUp() => popup.SetActive(true);

    private void SetPopUp(string sessionName = "Session")
    {
        switch (sessionName)
        {
            case "Intro":
                introPopup.SetActive(true);
                sessionPopup.SetActive(false);
                accidentPopup.SetActive(false);
                popup = introPopup;
                break;
            case "Accident":
                introPopup.SetActive(false);
                sessionPopup.SetActive(false);
                accidentPopup.SetActive(true);
                popup = accidentPopup;
                activeInteraction = true;
                break;
            default:
                // 인터렉션 활성화
                introPopup.SetActive(false);
                sessionPopup.SetActive(true);
                accidentPopup.SetActive(false);
                popup = sessionPopup;
                activeInteraction = true;
                break;
        }
        popupText.text = _session.text;
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
            switch (_session.gameObject.name)
            {
                case "Intro":
                    startPopup.SetActive(true);
                    break;
                case "Accident":
                    Invoke(nameof(ShowQuiz), startDelayTime);
                    AccidentDone();
                    break;
                case "Ending":
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

    private IEnumerator EndingProcess()
    {
        yield return new WaitForSeconds(5f);

        FadeBlack();
        
        yield return new WaitForSeconds(2f);
        
#if UNITY_ANDROID
        _questToHome.BackButtonToHome();
#else
        _viveToHome.BackButtonToHome();
#endif
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
