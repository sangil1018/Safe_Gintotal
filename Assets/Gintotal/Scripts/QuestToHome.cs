using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using UnityEditor;
using UnityEngine.SceneManagement;
using static UnityEngine.Application;
using Debug = UnityEngine.Debug;

public class QuestToHome : MonoBehaviour
{
    private const float QuitDelay = 2.0f; // 종료 대기 시간
    private bool _isAppLaunched;
    
    // [SerializeField] public Button toHomeBtn;
    private CanvasGroup _canvasGroup;
    private CanvasGroup _accidentGroup;
    private const string SceneName = "home";

    private Sequence _mySequence;
    
    private void Awake()
    {
        _canvasGroup = GameObject.Find("OverlayFader").GetComponent<CanvasGroup>();
        _accidentGroup = GameObject.Find("AccidentFader").GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 1;
        _accidentGroup.alpha = 0;
    }
    
    private void Start() => runInBackground = true;
    
    public void FaderSequence()
    {
        _mySequence = DOTween.Sequence()
            .OnStart(() => { _accidentGroup.DOFade(0, 1f).SetEase(Ease.InCubic); })
            .Append(_canvasGroup.DOFade(1, 3f).SetEase(Ease.InCubic));
    }

    public void CameraToWhite()
    {
        _canvasGroup.DOFade(0, 0.5f).SetEase(Ease.InCubic); // 시작할때 검->흰 페이드
    }

    public void CameraToBlack()
    {
        _canvasGroup.DOFade(1, 0.5f).SetEase(Ease.InCubic); // 시작할때 검->흰 페이드
    }

    public void BackButtonToHome() => LaunchHome();

    private void LaunchHome()
    {
        _canvasGroup.DOFade(1, 1f).SetEase(Ease.OutCubic); // 다른앱으로 넘어갈때 희->검 페이드
        
#if UNITY_EDITOR
        try
        {
            SceneManager.LoadScene(SceneName);
        }
        catch (Exception e)
        {
            Debug.LogError($"{SceneName} 씬을 찾을 수 없습니다." + e.Message);
        }
#else // 안드로이드 일때
        var apkName = $"com.gintotal.mc.{SceneName}";
        LaunchAndroidApp(apkName.ToLower());
#endif
    }
    
    private static void LaunchAndroidApp(string packageName)
    {
        try
        {
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                var packageManager = currentActivity.Call<AndroidJavaObject>("getPackageManager");
                // 다른 앱 실행
                var launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", packageName);
                if (launchIntent is not null)
                {
                    currentActivity.Call("startActivity", launchIntent);
                    // 실행 성공 시 딜레이 후 현재 앱 종료
                    QuitApp().Forget();
                }
                else
                {
                    Debug.Log("앱 실행 실패: " + packageName);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("앱 실행 중 오류 발생: " + e.Message);
        }
    }
    
    private static async UniTaskVoid QuitApp()
    {
#if UNITY_EDITOR
        await UniTask.Delay(TimeSpan.FromSeconds(QuitDelay));
        EditorApplication.isPlaying = false;
#else // 안드로이드 일때
        Application.Quit();
#endif
    }
}
