using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Unit {
  public class StateController
  {
    [SerializeField]
    public readonly static State REMAIN_STATE;
    public readonly static State ROTATE_STATE;
    public readonly static State FOCUS_ATTACK_STATE;

    static StateController()
    {
      REMAIN_STATE = Resources.Load<State>("Remain");
      ROTATE_STATE = Resources.Load<State>("Rotate");
      FOCUS_ATTACK_STATE = Resources.Load<State>("FocusAttack");
    }

    [SerializeField] [BoxGroup ("State")]
    public State CurrentState { get; private set; }
    [SerializeField] [Required(InfoMessageType.Error)] [BoxGroup ("State")]
    public bool IsAbleToAct => this.actionRemainingDelay <= 0;
    public bool IsAbleToTransition => this.transitionRemainingDelay <= 0;
    float actionRemainingDelay;
    float transitionRemainingDelay;

    public StateController(State initialState) {
      this.CurrentState = initialState;
    }

    [Button("Transition to new state")]
    public void TransitionTo(State nextState)
    {
      if (nextState != REMAIN_STATE) {
        this.OnExitState(this.CurrentState);
        this.CurrentState = nextState; 
        this.OnEnterState(nextState);
      }
    }

    void OnEnterState(State nextState)
    {
      this.actionRemainingDelay = nextState.ActionInterval;
      this.transitionRemainingDelay = nextState.TransitionInterval;
    }

    void OnExitState(State state)
    {
    }

    public void Update()
    {
      if (this.actionRemainingDelay > 0) {
        this.actionRemainingDelay -= Time.deltaTime;
      }
      if (this.transitionRemainingDelay > 0) {
        this.transitionRemainingDelay -= Time.deltaTime;
      }
    }

    public void ResetActionDelay()
    {
      this.actionRemainingDelay = this.CurrentState.ActionInterval;
    } 
  
    public void ResetTransitionDeley()
    {
      this.transitionRemainingDelay = this.CurrentState.TransitionInterval;
    }
  }
}
