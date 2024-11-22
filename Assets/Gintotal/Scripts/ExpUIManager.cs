using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ExpUIManager : MonoBehaviour
{
    [SerializeField] public Button backBtn;

    private ViveToHome _viveToHome;
    private QuestToHome _questToHome;
    private bool _vShow;

    public void OnMenu(InputAction.CallbackContext context) => SessionManager.Instance.ShowBackUI(_vShow = !_vShow);

    private void Start()
    {
#if UNITY_ANDROID
        _questToHome = GetComponent<QuestToHome>();
        backBtn.onClick.AddListener(_questToHome.BackButtonToHome);
#else
        _viveToHome = GetComponent<ViveToHome>();
        backBtn.onClick.AddListener(_viveToHome.BackButtonToHome);
#endif
    }

    public void ShowBackButton()
    {
        _vShow = true;
        
        Invoke(nameof(AutoGoHome), 5f);
    }

    private void AutoGoHome()
    {
        backBtn.onClick.Invoke();
        
        _vShow = false;
    }
}
