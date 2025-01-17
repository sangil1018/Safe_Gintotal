using UnityEngine;

public class EnableObjectsWhenHide : MonoBehaviour
{
    [SerializeField] private GameObject[] objects;

    private void OnDisable()
    {
        if (objects.Length <= 0) return;
        
        foreach (var obj in objects)
        {
            obj.SetActive(true);
        }

        enabled = false;
    }
}
