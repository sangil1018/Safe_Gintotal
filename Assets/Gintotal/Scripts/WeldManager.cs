using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WeldMananger : MonoBehaviour
{
    [SerializeField] private GameObject[] targets; // 타겟 스피어들
    [SerializeField] private GameObject[] weldGroups; // 타겟 스피어 그룹
    [SerializeField] private AudioSource audioGroups; // 타겟 사운드 그룹
    [SerializeField] private ParticleSystem particleFX; // 타겟 스피어 그룹
    
    private int _targetCount;      // 현재 충족된 타겟 개수
    private int _weldGroupCount;      // 현재 충족된 그룹 개수

    [SerializeField] private int targetLength = 7;
    [SerializeField] private float delayTime = 2f;

    private float _timer;
    private bool _isCollide;
    
    


    private void OnEnable() => ShowWeldGroup();

    public void ShowWeldGroup()
    {
        _targetCount = 0;
        weldGroups[_weldGroupCount].SetActive(true);
        FindChildTargets(_weldGroupCount);
    }

    private void FindChildTargets(int num)
    {
        var objs = new List<GameObject>();
        for (var i = 0; i < weldGroups[num].transform.childCount; i++)
        {
            var obj = weldGroups[num].transform.GetChild(i).gameObject;
            objs.Add(obj);
        }

        targets = objs.ToArray();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (targets[_targetCount].name == other.gameObject.name)
        {
            _isCollide = true;
            HandleCollision(targets[_targetCount]);
            other.enabled = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _isCollide = false;
    }

    private void Update()
    {
        if (_isCollide)
        {
            audioGroups.Play();
            particleFX.Play();
        }
        else
        {
            audioGroups.Pause();
            particleFX.Pause();
        }
    }

    private void HandleCollision(GameObject sphere)
    {
        var material = sphere.GetComponent<MeshRenderer>().material;
        material.DOColor(Color.red, 1f).OnComplete(() =>
        {
            sphere.SetActive(false); // Sphere 삭제
            _targetCount++;  // 카운터 증가

            if (_targetCount < targetLength)
            {
                targets[_targetCount].GetComponent<MeshRenderer>().material.color = Color.white;
                targets[_targetCount].GetComponent<SphereCollider>().enabled = true;
            }
            else
            {
                OnAllTargetsMet();
            }
        });
    }

    private void OnAllTargetsMet()
    {
        weldGroups[_weldGroupCount].SetActive(false);
        
        _weldGroupCount++;
        
        Invoke(_weldGroupCount < weldGroups.Length ? nameof(ShowWeldGroup) : nameof(WeldActionDone), delayTime);
    }

    private void WeldActionDone()
    {
        SessionManager.Instance.SessionDone();
        // 또는 파티클 등 이펙트 실행 시키고 넘어가기.
        // particleFX.SetActive(true);
    }
}
