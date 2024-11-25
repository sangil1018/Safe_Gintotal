using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartOnOff : MonoBehaviour
{
    [SerializeField] private GameObject[] hideObjs;
    [SerializeField] private GameObject[] showObjs;

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
