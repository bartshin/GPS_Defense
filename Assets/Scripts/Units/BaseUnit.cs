using UnityEngine;
using Architecture;
using System;
using Sirenix.OdinInspector;

namespace Unit
{
  public abstract class BaseUnit: _MonoBehaviour, IStateControlable, IPooledObject
  {
    [SerializeField]
    public bool IsActive { get; protected set; }
    [SerializeField]
    public Stat Stat { get; set; }
    [SerializeField]
    public ProjectileStat projectileStat;
    [SerializeField]
    public State DefaultState
    { 
      get => this.defaultState;
      set {
        this.defaultState = value;
        if (this.StateController != null) {
          this.StateController.TransitionTo(value);
        }
      }
    }
    State defaultState;
    public BaseDamagable Damagable { get; protected set; }
    [ShowInInspector]
    public StateController StateController { get; protected set; }
    public Action<IPooledObject> OnDisabled { get; set; }

    public void Reset()
    {
      if (this.Damagable != null) {
        var max = this.Damagable.Hp.Value.max;
        this.Damagable.Hp.Value = (max, max);
      } 
      if (this.StateController != null) {
        this.StateController.TransitionTo(this.DefaultState);
      }
    }

    public virtual void Init()
    {
      this.StateController = new StateController(this.DefaultState);
      if (this.Damagable != null) {
        this.Damagable.SetMaxHp(this.Stat.Hp);
      }
    }

    protected virtual void Awake()
    {
      if (this.Damagable == null) {
        this.Damagable = this.gameObject.GetComponent<BaseDamagable>();
      }
      if (this.Damagable == null) {
        this.Damagable = this.gameObject.AddComponent<BaseDamagable>();
      }
    }

    protected virtual void Update()
    {
      if (this.IsActive) {
        this.StateController.Update();
        if (this.StateController.IsAbleToAct) {
          this.StateController.CurrentState.PerformAction(this);
          this.StateController.ResetActionDelay();
        }
        if (this.StateController.IsAbleToTransition) {
          this.StateController.CurrentState.UpdateTransition(this);  
          this.StateController.ResetTransitionDeley();
        }
      }
    }

    protected virtual void OnDisable()
    {
      if (this.OnDisabled != null) {
        this.OnDisabled.Invoke(this);
      }
    }
  }
}
