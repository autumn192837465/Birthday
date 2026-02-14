using System;
using Coffee.UIEffects;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainViewIcon : MonoBehaviour
{
    [SerializeField] private MMF_Player pointerDownFeedback;
    [SerializeField] private MMF_Player pointerUpFeedback;
    [SerializeField] private UIEffect uiEffect;
    [SerializeField] private GameObject tooltips;

    [SerializeField] private Button button;
    
    public Action OnIconClicked;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        tooltips.SetActive(false);
        uiEffect.enabled = false;
        button.onClick.AddListener(() => OnIconClicked?.Invoke());
    }

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerDown()
    {
        if (pointerUpFeedback.IsPlaying)
        {
            pointerUpFeedback.StopFeedbacks();
        }
        
        pointerDownFeedback.PlayFeedbacks();
        uiEffect.enabled = true;
        tooltips.SetActive(true);
    }

    public void OnPointerUp()
    {
        if (pointerDownFeedback.IsPlaying)
        {
            pointerDownFeedback.StopFeedbacks();
        }
        
        pointerUpFeedback.PlayFeedbacks();
        uiEffect.enabled = false;
        tooltips.SetActive(false);
    }
}
