using UnityEngine;

public class CanvasFaderStatus : MonoBehaviour
{
    private CanvasGroup CanvasFader => GetComponent<CanvasGroup>();
    private void Awake()
    {
        CanvasFader.alpha = 1; // 검정화면
    }
}
