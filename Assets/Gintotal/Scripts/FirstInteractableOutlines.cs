using UnityEngine;

public class FirstInteractableOutlines : MonoBehaviour
{
    [SerializeField] private GameObject openedPopUp;
    [SerializeField] private Outline[] outlines;

    private void Update()
    {
        if (openedPopUp.activeSelf) return;
        if (outlines.Length > 0)
        {
            foreach (var outline in outlines)
            {
                outline.enabled = true;
            }

            enabled = false;
        }
    }
}
