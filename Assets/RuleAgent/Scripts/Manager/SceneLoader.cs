using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string sceneNameToLoad;

    public void LoadScene()
    {
        if (string.IsNullOrEmpty(sceneNameToLoad)) return;
        SceneManager.LoadScene(sceneNameToLoad);
    }
}