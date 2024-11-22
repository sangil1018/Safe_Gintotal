using UnityEngine;

public class EnableInteractableOutlines : MonoBehaviour
{
    [SerializeField] private Outline[] outlines;

    private void OnDisable()
    {
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
