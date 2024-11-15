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
    [SerializeField] public CanvasGroup cameraFade;
    [SerializeField] public Transform camPosition;
    [SerializeField] public GameObject interaction;
    [SerializeField] public TMP_Text noticeText;
    [SerializeField] public TMP_Text popupText;
    
    public PlayableDirector playableDirector;
    public int currentSessionID;
    
    private Session _session;
    private GameObject _uiManager;
    
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
        noticeText.transform.parent.parent.parent.parent.gameObject.SetActive(false);
        popupText.transform.parent.parent.parent.parent.gameObject.SetActive(false);
    }
    
    public static SessionManager Instance => null == _instance ? null : _instance;

    private void Start()
    {
        FindChildSessions();
        
        ShowIntro();

#if UNITY_ANDROID
        _uiManager.GetComponent<QuestToHome>().CameraToWhite();
#else
        _uiManager.GetComponent<ViveToHome>().CameraToWhite();
#endif
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

    public void ShowIntro()
    {
        intro.SetActive(true);
        _session = intro.GetComponent<Session>();
        
        ProcessingSession();
    }

    public void IntroDone()
    {
        ShowSession();
        
        intro.SetActive(false);
    }

    public void ShowSession()
    {
        if (currentSessionID >= sessions.Length)
        {
            ShowAccident();
            return;
        }
        
        _session = sessions[currentSessionID].GetComponent<Session>();
        
        ProcessingSession();
    }

    private void NextSession()
    {
        if (currentSessionID <= sessions.Length)
        {
            ShowSession();
        }
        else
        {
            // todo: 종료 버튼 또는 마지막을 알리는 요약 맨트?
        }
    }
    
    public void SessionDone()
    {
        currentSessionID++;
        
        if (_session.isDone) ShowAccident();
        else NextSession();
        
        sessions[currentSessionID-1].SetActive(false);
    }

    public void ShowAccident()
    {
        _session = accident.GetComponent<Session>();
        
        ProcessingSession();
    }

    public void AccidentDone()
    {
        ShowQuiz();
        
        accident.SetActive(false);
    }

    public void ShowQuiz()
    {
        _session = quiz.GetComponent<Session>();
        
        ProcessingSession();
    }
    
    public void QuizDone()
    {
        // todo: 엔딩 관련 무언가 하던지 아니면
        _uiManager.GetComponent<ExpUIManager>().ShowBackButton();
        
        quiz.SetActive(false);
    }

    private void ProcessingSession()
    {
        interaction.SetActive(_session.isInteractable);
        _session.GetDirector();
        
        if (_session.isPopup)
        {
            noticeText.text = _session.text;
            noticeText.transform.parent.parent.parent.parent.gameObject.SetActive(true);
        }
        
        else if (_session.isAnim)
        {
            playableDirector.Play();
        }
    }
}
