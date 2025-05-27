using System;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackUI : MonoBehaviour
{
    /// <summary>
    /// +1(Good)か -1(Bad)を投げるイベント
    /// </summary>
    public static event Action<int> OnFeedback;

    [SerializeField] private Button goodButton;
    [SerializeField] private Button badButton;

    private void Start()
    {
        if (goodButton == null || badButton == null)
        {
            Debug.LogError("FeedbackUI:Buttonがアサインされていません");
            return;
        }
        
        goodButton.onClick.AddListener(()=>OnFeedback?.Invoke(+1));
        badButton.onClick.AddListener(()=>OnFeedback?.Invoke(-1));
    }
}
