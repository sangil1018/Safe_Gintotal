using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ExpUIManager : MonoBehaviour
{
    [SerializeField] public Button backBtn;

    private GameObject _backBtnUI;
    private ViveToHome _viveToHome;
    private QuestToHome _questToHome;
    private bool _vShow;

    public void OnMenu(InputAction.CallbackContext context) => _vShow = !_vShow;

    private void Awake()
    {
        _backBtnUI = backBtn.transform.parent.parent.gameObject;
    }

    private void Start()
    {
#if UNITY_ANDROID
        _questToHome = GetComponent<QuestToHome>();
        backBtn.onClick.AddListener(_questToHome.BackButtonToHome);
#else
        _viveToHome = GetComponent<ViveToHome>();
        backBtn.onClick.AddListener(_viveToHome.BackButtonToHome);
#endif
        
        _backBtnUI.SetActive(false);
    }

    private void Update()
    {
        _backBtnUI.SetActive(_vShow);
    }

    public void ShowBackButton()
    {
        _vShow = true;
        
        Invoke(nameof(AutoGoHome), 5f);
    }

    private void AutoGoHome()
    {
        backBtn.onClick.Invoke();
    }
}
