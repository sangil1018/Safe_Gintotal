using System;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class AppLauncher : MonoBehaviour
{
    private const string WindowsDirPath = @"C:\Gintotal\mc"; // 윈도우 exe 경로
    private const float QuitDelay = 2.0f; // 종료 대기 시간
    private bool _isAppLaunched;
    
    public Button[] expBtns;    // 이름에 'exp'를 포함하는 버튼 배열
    public Button[] vizBtns;    // 이름에 'viz'를 포함하는 버튼 배열

    [SerializeField] public GameObject[] menuItem1, menuItem2, menuItem3, menuItem4;
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GameObject.Find("OverlayFader").GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        FindAndCategorizeButtons();
        Application.runInBackground = true;
    }

    private void FindAndCategorizeButtons()
    {
        // 모든 버튼을 임시 리스트에 저장
        var expList = new List<Button>();
        var vizList = new List<Button>();

        // 씬 내 모든 버튼을 검색
        var allButtons = FindObjectsOfType<Button>();

        foreach (var btn in allButtons)
        {
            var btnName = btn.transform.parent.name; // 버튼 이름을 소문자로 변환하여 비교

            if (btnName.Contains("Exp"))
            {
                expList.Add(btn);
                btn.onClick.AddListener(() => LaunchApp(btnName));
            }
            else if (btnName.Contains("Viz"))
            {
                vizList.Add(btn);
                btn.onClick.AddListener(() => LaunchApp(btnName));
            }
        }

        // 리스트를 배열로 변환
        expBtns = expList.ToArray();
        vizBtns = vizList.ToArray();

        HideUIStartMenu();
        _canvasGroup.DOFade(0, 2f).SetEase(Ease.InCubic); // 시작할때 검->흰 페이드
        // Debug.Log($"EXP Buttons: {expBtns.Length}, VIZ Buttons: {vizBtns.Length}");
    }
    
    private void HideUIStartMenu()
    {
#if UNITY_EDITOR
        if (EditorUserBuildSettings.activeBuildTarget is BuildTarget.Android)
        {
            menuItem1[1].SetActive(false);
            menuItem2[1].SetActive(false);
            menuItem3[1].SetActive(false);
            menuItem4[1].SetActive(false);
        }
        else
        {
            menuItem1[0].SetActive(false);
            menuItem2[0].SetActive(false);
            menuItem3[0].SetActive(false);
            menuItem4[0].SetActive(false);
        }
#elif UNITY_ANDROID // 안드로이드 일때
        menuItem1[1].SetActive(false);
        menuItem2[1].SetActive(false);
        menuItem3[1].SetActive(false);
        menuItem4[1].SetActive(false);
#else // 윈도우 일때
        menuItem1[0].SetActive(false);
        menuItem2[0].SetActive(false);
        menuItem3[0].SetActive(false);
        menuItem4[0].SetActive(false);
#endif
    }
    
    private void LaunchApp(string sceneName)
    {
        _canvasGroup.DOFade(1, 1f).SetEase(Ease.OutCubic); // 다른앱으로 넘어갈때 희->검 페이드
#if UNITY_EDITOR
        try
        {
            SceneManager.LoadScene(sceneName);
        }
        catch (Exception e)
        {
            Debug.LogError($"{sceneName} 씬을 찾을 수 없습니다." + e.Message);
        }
        
        // if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
        // {
        //     var apkName = $"com.mc.{sceneName}";
        //     LaunchAndroidApp(apkName);
        // }
        // else
        // {
        //     var exePath = @$"{WindowsDirPath}\{sceneName}\{sceneName}.exe";
        //     LaunchWindowsApp(exePath);
        // }
#elif UNITY_ANDROID // 안드로이드 일때
        var apkName = $"com.mc.{sceneName}";
        LaunchAndroidApp(apkName);
#else // 윈도우 일때
        var exePath = @$"{windowsDirPath}\{sceneName}\{sceneName}.exe";
        LaunchWindowsApp(exePath);
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
#elif UNITY_ANDROID // 안드로이드 일때
        Application.Quit();
#else // 윈도우 일때
        await UniTask.Delay(TimeSpan.FromSeconds(QuitDelay));
        Application.Quit();
#endif
    }
}
