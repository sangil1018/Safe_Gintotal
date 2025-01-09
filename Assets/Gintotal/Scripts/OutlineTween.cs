using System;
using UnityEngine;
using EPOOutline;
using DG.Tweening;

public class OutlineTween : MonoBehaviour
{
    private Outlinable _outline;
    private float _startTime = 1;

    private void OnEnable()
    {
        _outline = GetComponent<Outlinable>();
        _outline.FrontParameters.DODilateShift(0, 1).SetLoops(-1, LoopType.Yoyo);
    }

    private void Update()
    {
        _startTime += Time.deltaTime;
        _outline.FrontParameters.FillPass.SetColor("_PublicColor",
            new Color(1, 0, 0, Mathf.PingPong(_startTime, 1) * 0.2f));
    }
}
