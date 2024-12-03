using System;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using UnityEditor;
using UnityEngine.SceneManagement;
using static UnityEngine.Application;
using Debug = UnityEngine.Debug;

public class ViveToHome : MonoBehaviour
{
    private const string WindowsDirPath = @"C:\Gintotal\mc"; // 윈도우 exe 경로
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
#if !UNITY_ANDROID
        _canvasGroup.alpha = 1;
        _accidentGroup.alpha = 0;
#endif
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
#else // 윈도우 일때
        var exePath = @$"C:\Gintotal\Vive\{SceneName}\{SceneName}.exe";
        LaunchWindowsApp(exePath);
#endif
    }
    
    private static void LaunchWindowsApp(string exeFilePath)
    {
        try
        {
            var process = Process.Start(exeFilePath);
            if (process is { HasExited: false })
            {
                // 실행 성공 시 딜레이 후 현재 앱 종료
                QuitApp().Forget();
            }
            else
            {
                Debug.Log("앱 실행 실패: " + exeFilePath);
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
#else // 윈도우 일때
        await UniTask.Delay(TimeSpan.FromSeconds(QuitDelay));
        Application.Quit();
#endif
    }
}
