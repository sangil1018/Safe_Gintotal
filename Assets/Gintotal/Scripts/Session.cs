using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.XR;

public class Session : MonoBehaviour
{
    [TextArea]
    public string text = "시작과 관련된\n텍스트를 적습니다.\n세줄까지 정렬 가능합니다.\n추가 텍스트";
    public bool isAnim = true;
    public bool startAnim = true;
    public bool isPopup = true;
    public bool isStartPosition = true;
    public bool isDone = true;
    public bool goToAccident;
    public bool nextSession;

    private XROrigin _xrOrigin;
    // public InputActionAsset inputActions; // InputActionAsset 참조 (XR Interaction Input)

    private void OnEnable()
    {
        // inputActions = SessionManager.Instance.GetInputActionManager().actionAssets[0];
    }

    public void SetStartingPosition()
    {
        if (!isStartPosition) return;
        // _xrOrigin = SessionManager.Instance.playerOrigin.GetComponent<XROrigin>();
        var transform1 = transform;
        SessionManager.Instance.playerOrigin.SetPositionAndRotation(transform1.position, transform1.rotation);
        // _xrOrigin.MoveCameraToWorldLocation(transform.position);
        // _xrOrigin.transform.localRotation = transform.localRotation;
        
        RefreshControllers();
    }
    
    public void RefreshControllers()
    {
        // XR Input Subsystem을 통해 TryRecenter 호출
        if (TryRecenterOrigin())
        {
            Debug.Log("리센터 성공!");
        }
        else
        {
            Debug.LogWarning("리센터 실패 또는 지원되지 않음.");
        }

        // XR Interaction Toolkit과 컨트롤러 액션 동기화
        RefreshControllersAndRecenter();
    }

    private void RefreshControllersAndRecenter()
    {
        // XR Input Subsystem에서 TryRecenter 호출
        if (TryRecenterOrigin())
        {
            Debug.Log("리센터 성공!");
        }
        else
        {
            Debug.LogWarning("리센터 실패 또는 지원되지 않음.");
        }

        // Input System 리프레시
        // RefreshInputActions();
    }

    private bool TryRecenterOrigin()
    {
        // 활성화된 XR Input Subsystem 가져오기
        var subsystems = new List<XRInputSubsystem>();
        SubsystemManager.GetInstances(subsystems);

        foreach (var subsystem in subsystems)
        {
            if (subsystem.TryRecenter())
            {
                subsystem.Stop();
                subsystem.Start();
                return true; // 리센터 성공
            }
        }
        return false; // 리센터 실패
    }

    // public void RefreshInputActions()
    // {
    //     if (inputActions == null)
    //     {
    //         Debug.LogWarning("InputActionAsset이 설정되지 않았습니다.");
    //         return;
    //     }
    //
    //     // InputActionAsset을 비활성화 후 활성화
    //     inputActions.Disable();
    //     inputActions.Enable();
    //
    //     Debug.Log("Input System 액션이 리프레시되었습니다.");
    // }

    public void GetDirector()
    {
        if (gameObject.name.Contains("Quiz")) return;
        SessionManager.Instance.playableDirector = GetComponent<PlayableDirector>();
    }
}
