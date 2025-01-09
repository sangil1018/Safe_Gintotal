using UnityEngine;

public class ControllerInteractorShow : MonoBehaviour
{
    [SerializeField] private GameObject interactorPrefabPoke;
    [SerializeField] private GameObject interactorPrefab;

    private void OnEnable()
    {
        interactorPrefab.SetActive(false);
        interactorPrefabPoke.SetActive(false);
    }

    private void OnDisable()
    {
        interactorPrefab.SetActive(true);
        interactorPrefabPoke.SetActive(true);
    }
}
