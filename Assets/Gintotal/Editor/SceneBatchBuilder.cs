using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEditor.Build.Reporting;

public class SceneBatchBuilder : MonoBehaviour
{
    private const string ScenesFolderPath = @"Assets\Gintotal\Scenes"; // 씬 폴더 경로
    private const string OutputFolder = @"C:\Gintotal\mc"; // 빌드 출력 폴더 경로

    [MenuItem("Build/Build Scenes to APKs")]
    public static void APKBuildScenes()
    {
        // 씬 검색
        var sceneGUIDs = AssetDatabase.FindAssets("t:Scene", new[] { ScenesFolderPath });

        foreach (var sceneGUID in sceneGUIDs)
        {
            // 씬 파일 경로 및 이름 가져오기
            var scenePath = AssetDatabase.GUIDToAssetPath(sceneGUID);
            var sceneName = Path.GetFileNameWithoutExtension(scenePath);
            var sceneFolder = Path.GetDirectoryName(scenePath);

            // 빌드 대상 및 설정 초기화
            BuildTarget buildTarget;
            var packageName = $"com.mc.{sceneName}";

            // 폴더에 따라 플랫폼 분기
            if (sceneFolder != null && !sceneFolder.Contains("Quest"))
            {
                buildTarget = BuildTarget.StandaloneWindows64;
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Standalone, packageName);
            }
            else
            {
                Debug.LogWarning($"'{sceneName}' 씬은 지원되지 않는 폴더에 있습니다.");
                continue;
            }

            // 프로덕트 이름 설정
            PlayerSettings.productName = sceneName;

            // 빌드 출력 경로 및 파일 확장자 설정
            var sceneOutputFolder = Path.Combine(OutputFolder, sceneName);
            Directory.CreateDirectory(sceneOutputFolder); // 폴더가 없으면 생성

            var buildFileName = sceneName + ".apk";
            var buildPath = Path.Combine(sceneOutputFolder, buildFileName);

            // 빌드 옵션 설정
            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new[] { scenePath }, // 단일 씬 빌드
                locationPathName = buildPath,
                target = buildTarget,
                options = BuildOptions.None
            };

            // 빌드 실행
            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            var summary = report.summary;

            // 빌드 결과 출력
            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"'{sceneName}' 씬 빌드 성공: {buildPath}");
            }
            else
            {
                Debug.LogError($"'{sceneName}' 씬 빌드 실패: {summary.totalErrors} 에러");
            }
        }
    }
    
    [MenuItem("Build/Build Scenes to EXEs")]
    public static void EXEBuildScenes()
    {
        // 씬 검색
        var sceneGUIDs = AssetDatabase.FindAssets("t:Scene", new[] { ScenesFolderPath });

        foreach (var sceneGUID in sceneGUIDs)
        {
            // 씬 파일 경로 및 이름 가져오기
            var scenePath = AssetDatabase.GUIDToAssetPath(sceneGUID);
            var sceneName = Path.GetFileNameWithoutExtension(scenePath);
            var sceneFolder = Path.GetDirectoryName(scenePath);

            // 빌드 대상 및 설정 초기화
            BuildTarget buildTarget;
            var packageName = $"com.mc.{sceneName}";

            // 폴더에 따라 플랫폼 분기
            if (sceneFolder != null && !sceneFolder.Contains("Vive"))
            {
                buildTarget = BuildTarget.Android;
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, packageName);
            }
            else
            {
                Debug.LogWarning($"'{sceneName}' 씬은 지원되지 않는 폴더에 있습니다.");
                continue;
            }

            // 프로덕트 이름 설정
            PlayerSettings.productName = sceneName;

            // 빌드 출력 경로 및 파일 확장자 설정
            var sceneOutputFolder = Path.Combine(OutputFolder, sceneName);
            Directory.CreateDirectory(sceneOutputFolder); // 폴더가 없으면 생성

            var buildFileName = sceneName + ".exe";
            var buildPath = Path.Combine(sceneOutputFolder, buildFileName);

            // 빌드 옵션 설정
            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new[] { scenePath }, // 단일 씬 빌드
                locationPathName = buildPath,
                target = buildTarget,
                options = BuildOptions.None
            };

            // 빌드 실행
            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            var summary = report.summary;

            // 빌드 결과 출력
            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"'{sceneName}' 씬 빌드 성공: {buildPath}");
            }
            else
            {
                Debug.LogError($"'{sceneName}' 씬 빌드 실패: {summary.totalErrors} 에러");
            }
        }
    }
}
