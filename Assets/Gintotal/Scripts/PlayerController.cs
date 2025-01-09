using UnityEngine;
// using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Transform resetTransform;
    [SerializeField] private GameObject player;
    [SerializeField] private Camera playerHead;

    // [SerializeField] private InputActionProperty recenterButton;

    [ContextMenu("Reset Position")]
    public void ResetPosition()
    {
        var rotationAngleY = resetTransform.rotation.eulerAngles.y - playerHead.transform.rotation.eulerAngles.y;
        player.transform.Rotate(0, rotationAngleY, 0);
        
        var distanceDiff = resetTransform.position - playerHead.transform.position;
        player.transform.position += distanceDiff;
    }
    
    private void Update()
    {
        // if (recenterButton.action.WasPressedThisFrame())
        // {
        //     ResetPosition();
        // }
    }
}
