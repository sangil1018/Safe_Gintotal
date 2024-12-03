using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenSubScene : MonoBehaviour
{
    public void GoSubScene(string subScene)
    {
        SceneManager.LoadScene(subScene);
    }
}
