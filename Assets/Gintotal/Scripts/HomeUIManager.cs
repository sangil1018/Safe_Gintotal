using UnityEditor;
using UnityEngine;

public class HomeUIManager : MonoBehaviour
{
    [SerializeField] public GameObject vive, quest;

    private void Awake()
    {
#if UNITY_EDITOR
        vive.SetActive(EditorUserBuildSettings.activeBuildTarget is not BuildTarget.Android); 
        quest.SetActive(EditorUserBuildSettings.activeBuildTarget is BuildTarget.Android);
#elif UNITY_ANDROID // 안드로이드 일때
        vive.SetActive(false);
        quest.SetActive(true);
#else // 윈도우 일때
        vive.SetActive(true);
        quest.SetActive(false);
#endif
    }
}