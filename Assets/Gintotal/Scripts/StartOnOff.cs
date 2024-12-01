using UnityEngine;

public class StartOnOff : MonoBehaviour
{
    [SerializeField] private GameObject[] hideObjs;
    [SerializeField] private GameObject[] showObjs;
    [SerializeField] private Outline[] outlines;

    private void OnEnable()
    {
        if (hideObjs.Length > 0)
        {
            foreach (var hObj in hideObjs)
            {
                hObj.SetActive(false);
            }
        }
        
        if (showObjs.Length > 0)
        {
            foreach (var sObj in showObjs)
            {
                sObj.SetActive(true);
            }
        }
        
        if (outlines.Length > 0)
        {
            foreach (var line in outlines)
            {
                line.enabled = true;
            }
        }
    }
    
    // private void OnDisable()
    // {
    //     foreach (var hObj in hideObjs)
    //     {
    //         hObj.SetActive(true);
    //     }
    //
    //     foreach (var sObj in showObjs)
    //     {
    //         sObj.SetActive(false);
    //     }
    // }
}
