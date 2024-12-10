using UnityEngine;
using UnityEngine.Animations;

public class ConstrainControl : MonoBehaviour
{
    [SerializeField] private ParentConstraint parentConstraint;

    private void Awake()
    {
        // _parentConstraint = SessionManager.Instance.playerOrigin.GetComponent<ParentConstraint>();
    }
    
    private void OnEnable()
    {
        // parentConstraint = SessionManager.Instance.playerOrigin.GetComponent<ParentConstraint>();
        parentConstraint.constraintActive = false;
    }

    private void OnDisable()
    {
        // parentConstraint.constraintActive = true;
    }

    public void OnConstrain()
    {
        parentConstraint.constraintActive = true;
    }

    public void OnConstrainBack()
    {
        parentConstraint.constraintActive = true;
        var offsetTransform = parentConstraint.transform;
        offsetTransform.localPosition = Vector3.zero;
        offsetTransform.localEulerAngles = Vector3.zero;
        parentConstraint.locked = true;
    }
    
    public void OffConstrain()
    {
        parentConstraint.constraintActive = false;
        parentConstraint.locked = false;
    }

    public void OffConstrainZero()
    {
        parentConstraint.constraintActive = false;
        var offsetTransform = parentConstraint.transform;
        offsetTransform.localPosition = Vector3.zero;
        offsetTransform.localEulerAngles = Vector3.zero;
        parentConstraint.locked = false;
    }
}
