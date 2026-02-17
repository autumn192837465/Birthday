using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorBase : MonoBehaviour
{
    public enum State
    {
        Opening,
        Idle,
        Closing,
        Closed,
    }
    
    [SerializeField] private MMF_Player openSoundFeedbacks;
    [SerializeField] private MMF_Player closeSoundFeedbacks;
    Animator animator;
    
    public Action OnOpen;
    public Action OnOpened;
    public Action OnIdle;
    public Action OnClosed;
    public bool IsIdle => currentState == State.Idle;
    public bool IsOpened => currentState != State.Closed && currentState != State.Closing;


    [HideInInspector] private string openTrigger = "open";
    [HideInInspector] private string closeTrigger = "close";
    public State CurrentState => currentState;
    private State currentState = State.Closed;
    
    
    protected virtual void Awake()
    {
        GetAnimator();
    }

    public virtual void Open(Action action)
    {                
        if (IsOpened)    return;

        currentState = State.Opening;
        OnOpen = action;
        animator.SetTrigger(openTrigger);
        openSoundFeedbacks?.PlayFeedbacks();    
    }
    public virtual void Open()
    {
        if (IsOpened) return;
        
        currentState = State.Opening;
        animator.SetTrigger(openTrigger);
        openSoundFeedbacks?.PlayFeedbacks();
    }
    
    public void OnOpenEvent()
    {
        OnOpen?.Invoke();
    }
    public void OnOpenedEvent()
    {
        OnOpened?.Invoke();
    }
    public void OnIdleEvent()
    {
        OnIdle?.Invoke();
        currentState = State.Idle;
    }

    public virtual void Close()
    {
        if(!IsOpened)    return;
        
        currentState = State.Closing;
        animator.SetTrigger(closeTrigger);
        closeSoundFeedbacks?.PlayFeedbacks();
    }
    public virtual void Close(Action action)
    {        
        if (!IsOpened)   return;
        
        OnClosed = action;
        currentState = State.Closing;
        animator.SetTrigger(closeTrigger);
        closeSoundFeedbacks?.PlayFeedbacks();
            
    }
    public void OnClosedEvent()
    {
        OnClosed?.Invoke();
        currentState = State.Closed;
    }

    private void GetAnimator()
    {
        animator = GetComponent<Animator>();
    }
    public void ClearAllAction()
    {
        OnOpen = null;
        OnIdle = null;
        OnOpened = null;
        OnClosed = null;
    }

    private void OnValidate()
    {
        if(GetComponent<Animator>() == null)
        {
            gameObject.AddComponent<Animator>();
        }
    }
}
