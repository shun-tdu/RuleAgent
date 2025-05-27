using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootController : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnBeforeSceneLoad()
    {
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            if (CurrencyManager.I == null)
            {
                var go = new GameObject("CurrencyManager");
                go.AddComponent<CurrencyManager>();
            }

            if (ScoreManager.I == null)
            {
                var go = new GameObject("ScoreManager");
                go.AddComponent<ScoreManager>();
            }
        };
        
        //シングルトン系を実行
        SceneManager.LoadScene("StartMenuScene");
    }
}