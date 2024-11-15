using UnityEngine;
using UnityEngine.Video;

public class VRVideoSetup : MonoBehaviour
{
    private VideoPlayer _vp;
    [SerializeField] public VideoClip videoClip;         // 사용할 VideoClip
    [SerializeField] public Material skyboxMaterial;     // Skybox Material (VR_Video 메터리얼)
    [SerializeField] public GameObject xrOrigin;
    [SerializeField] public RenderTexture rt;
    private static readonly int ImageType = Shader.PropertyToID("_ImageType");
    private static readonly int Layout = Shader.PropertyToID("_Layout");

    private void Awake()
    {
        _vp = GetComponent<VideoPlayer>();
        videoClip = _vp.clip;
        rt.width = (int)videoClip.width;
        rt.height = (int)videoClip.height;
        SetupSkyboxBasedOnVideo();
    }

    private void SetupSkyboxBasedOnVideo()
    {
        if (videoClip == null || skyboxMaterial == null)
        {
            Debug.LogError("VideoClip 또는 Skybox Material이 설정되지 않았습니다.");
            return;
        }

        // Skybox Material을 VR_Video Material로 설정
        RenderSettings.skybox = skyboxMaterial;

        // VideoClip 이름을 가져와서 소문자로 변환
        var videoName = videoClip.name.ToLower();
        var nameParts = videoName.Split('_');

        if (nameParts.Length < 3)
        {
            Debug.LogWarning("VideoClip 이름 형식이 올바르지 않습니다. 형식: name_360_or_180_none_or_sbs");
            return;
        }

        // 1. 파일 이름에 따라 Image Type 설정
        if (nameParts[1] =="360")
        {
            xrOrigin.transform.localEulerAngles = new Vector3(0, 90, 0);
            skyboxMaterial.SetFloat(ImageType, 0);  // 360도 이미지 타입으로 설정
            Debug.Log("Skybox Image Type을 360으로 설정");
        }
        else if (nameParts[1] =="180")
        {
            xrOrigin.transform.localEulerAngles = Vector3.zero;
            skyboxMaterial.SetFloat(ImageType, 1);  // 180도 이미지 타입으로 설정
            Debug.Log("Skybox Image Type을 180으로 설정");
        }
        else
        {
            Debug.LogWarning("알 수 없는 Image Type. 360 또는 180으로 설정해야 합니다.");
        }

        // 2. 파일 이름에 따라 3D Layout 설정
        if (nameParts[2] == "sbs")
        {
            skyboxMaterial.SetFloat(Layout, 1);   // SBS 레이아웃으로 설정
            Debug.Log("Skybox 3D Layout을 SBS로 설정");
        }
        else if (nameParts[2] =="none")
        {
            skyboxMaterial.SetFloat(Layout, 0);   // None 레이아웃으로 설정
            Debug.Log("Skybox 3D Layout을 None으로 설정");
        }
        else
        {
            Debug.LogWarning("알 수 없는 3D Layout. None 또는 SBS로 설정해야 합니다.");
        }
    }
}