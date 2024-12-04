using System;
using UnityEngine;

public class WeldMananger1 : MonoBehaviour
{
    [SerializeField] private string targetTag = "Target"; // 트리거 대상 오브젝트의 태그
    [SerializeField] private float requiredTime = 5.0f;  // 겹쳐야 하는 시간
    [SerializeField] private float delayTime = 2f;
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private GameObject particle2;
    
    private float _overlapTimer = 0.0f; // 현재 겹친 시간
    private bool _isOverlapping = false; // 겹치고 있는 상태
    private Collider _collider;

    [SerializeField] private GameObject changeObj;
    [SerializeField] private bool change;
    private Color setColor;
    private int totalCount;


    private void Update()
    {
        if (_isOverlapping)
        {
            particle.Play();
            // 트리거 중일 때 타이머 증가
            _overlapTimer += Time.deltaTime;
            
            if (_overlapTimer / requiredTime > 0.6f && totalCount == 1)
            {
                particle2.SetActive(true);
            }

            if (_overlapTimer >= requiredTime)
            {
                Invoke(nameof(WeldActionDone), delayTime); // 조건 충족 시 메서드 실행
                _overlapTimer = 0.0f; // 타이머 리셋
            }

            setColor.r = _overlapTimer / requiredTime;
        }
        else
        {
            particle.Stop();
            // 트리거가 해제된 상태에서 타이머 유지
            _overlapTimer = Mathf.Max(0.0f, _overlapTimer); // 음수 방지
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            _collider = other;
            setColor = _collider.GetComponent<MeshRenderer>().material.color;
            _isOverlapping = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            _isOverlapping = false;
        }
    }

    private void WeldActionDone()
    {
        _isOverlapping = false;
        totalCount++;
        
        if (_collider != null)
        {
            _collider.transform.GetComponent<Rigidbody>().isKinematic = false;
            _collider = null;
        }

        SessionManager.Instance.SessionDone();
        
        if (!change) return;
        changeObj.SetActive(true);
        gameObject.SetActive(false);
        // 또는 파티클 등 이펙트 실행 시키고 넘어가기.
        // particleFX.SetActive(true);
    }
}
