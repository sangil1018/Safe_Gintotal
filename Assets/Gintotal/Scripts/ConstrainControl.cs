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
    
    public void OffConstrain()
    {
        parentConstraint.constraintActive = false;
    }
}
