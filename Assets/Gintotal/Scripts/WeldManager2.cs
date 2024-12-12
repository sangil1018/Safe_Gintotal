using UnityEngine;

public class WeldMananger2 : MonoBehaviour
{
    [SerializeField] private string targetTag = "Target"; // 트리거 대상 오브젝트의 태그
    [SerializeField] private float requiredTime = 5.0f;  // 겹쳐야 하는 시간
    [SerializeField] private float delayTime = 2f;
    
    private float _overlapTimer = 0.0f; // 현재 겹친 시간
    private bool _isOverlapping = false; // 겹치고 있는 상태
    private Collider _collider;

    [SerializeField] private GameObject changeObj;
    // [SerializeField] private GameObject hideObj;
    [SerializeField] private bool change;
    // private int totalCount;

    private void Update()
    {
        if (_isOverlapping)
        {
            // 트리거 중일 때 타이머 증가
            _overlapTimer += Time.deltaTime;

            if (_overlapTimer >= requiredTime)
            {
                Invoke(nameof(WeldActionDone), delayTime); // 조건 충족 시 메서드 실행
                _overlapTimer = 0.0f; // 타이머 리셋
            }
        }
        else
        {
            // 트리거가 해제된 상태에서 타이머 유지
            _overlapTimer = Mathf.Max(0.0f, _overlapTimer); // 음수 방지
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            _collider = other;
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
        // totalCount++;

        if (_collider != null)
        {
            _collider.gameObject.SetActive(false);
        }
        changeObj.SetActive(true);
        gameObject.SetActive(false);

        SessionManager.Instance.SessionDone();
    }
}
