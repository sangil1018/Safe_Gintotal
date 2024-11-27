using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WeldMananger : MonoBehaviour
{
    [SerializeField] private GameObject[] targets; // 타겟 스피어들
    [SerializeField] private GameObject[] weldGroups; // 타겟 스피어 그룹
    [SerializeField] private GameObject particleFX; // 타겟 스피어 그룹
    
    private int _targetCount;      // 현재 충족된 타겟 개수
    private int _weldGroupCount;      // 현재 충족된 그룹 개수

    [SerializeField] private int targetLength = 7;
    [SerializeField] private float delayTime = 2f;

    private float _timer;
    private bool _collide;

    // private void Start()
    // {
    //     // 1. Target Sphere에 Collider 추가
    //     foreach (var sphere in targetSpheres)
    //     {
    //         if (sphere != null)
    //         {
    //             var sphereCollider = sphere.GetComponent<SphereCollider>();
    //             if (sphereCollider == null)
    //             {
    //                 sphereCollider = sphere.AddComponent<SphereCollider>();
    //             }
    //             sphereCollider.isTrigger = false; // 기본 설정: Trigger 아님
    //         }
    //     }
    //
    //     // 2. Source Object에 Trigger Collider 설정
    //     var sourceCollider = sourceObject.GetComponent<Collider>();
    //     if (sourceCollider == null)
    //     {
    //         sourceCollider = sourceObject.AddComponent<BoxCollider>(); // 기본 BoxCollider 추가
    //     }
    //     sourceCollider.isTrigger = true; // Trigger로 설정
    // }

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
            HandleCollision(targets[_targetCount]);
            other.enabled = false;
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
                targets[_targetCount].GetComponent<MeshRenderer>().material.color = Color.yellow;
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
