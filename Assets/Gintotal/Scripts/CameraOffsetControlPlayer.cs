using UnityEngine.InputSystem;
using UnityEngine;

public class CameraOffsetControlPlayer : MonoBehaviour
{
    [SerializeField] private float offsetDelta = 0.05f;
    [SerializeField] private PlayerController playerController;
    
    private XRIDefaultInputActions _actionAsset;
    private Vector3 _position;
    
    private void Awake()
    {
        // Input Actions 초기화
        _actionAsset = new XRIDefaultInputActions();
        _actionAsset.UIActions.CameraFront.performed += OnOffsetFront;
        _actionAsset.UIActions.CameraBack.performed += OnOffsetBack;
        _actionAsset.UIActions.CameraLeft.performed += OnOffsetLeft;
        _actionAsset.UIActions.CameraRight.performed += OnOffsetRight;
        _actionAsset.UIActions.CameraUp.performed += OnOffsetUp;
        _actionAsset.UIActions.CameraDown.performed += OnOffsetDown;
        _actionAsset.UIActions.OffsetReset.performed += OnOffsetReset;
    }

    // Input Actions 활성화
    private void OnEnable() => _actionAsset.UIActions.Enable();

    // Input Actions 비활성화
    private void OnDisable() => _actionAsset.UIActions.Disable();

    private void OnOffsetFront(InputAction.CallbackContext ctx)
    {
        _position.z += offsetDelta;
        transform.localPosition = _position;
    }
    private void OnOffsetBack(InputAction.CallbackContext ctx)
    {
        _position.z -= offsetDelta;
        transform.localPosition = _position;
    }
    private void OnOffsetLeft(InputAction.CallbackContext ctx)
    {
        _position.x -= offsetDelta;
        transform.localPosition = _position;
    }
    private void OnOffsetRight(InputAction.CallbackContext ctx)
    {
        _position.x += offsetDelta;
        transform.localPosition = _position;
    }
    private void OnOffsetUp(InputAction.CallbackContext ctx)
    {
        _position.y -= offsetDelta;
        transform.localPosition = _position;
    }
    private void OnOffsetDown(InputAction.CallbackContext ctx)
    {
        _position.y += offsetDelta;
        transform.localPosition = _position;
    }
    private void OnOffsetReset(InputAction.CallbackContext ctx)
    {
        _position = Vector3.zero;
        transform.localPosition = _position;
        playerController.ResetPosition();
    }
}
