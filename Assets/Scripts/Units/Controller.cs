using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

namespace Unit
{
  public class Controller : _MonoBehaviour
  { 
    [SerializeField] [Required(InfoMessageType.Error)] 
    public Transform Eye { get; private set; }
    [SerializeField] [BoxGroup ("State")]
    public State CurrentState { get; private set; }
    [SerializeField] [Required(InfoMessageType.Error)] [BoxGroup ("State")]
    State RemainState;
    [HideInInspector]
    public NavMeshAgent navMeshAgent { get; private set; }
    [SerializeField]
    public Data Data { get; private set; }
    [ShowInInspector]
    public List<Vector3> WayPoints { get; private set; }
    [SerializeField]
    public Vector3 Shelther { get; private set; }
    [SerializeField]
    public bool IsActive { get; private set; }
    [HideInInspector]
    public int nextWayPoint;
    public BaseDamagable Damagable;
    public BaseDamagable ChaseTarget;
    float actionRemainingDelay;
    float transitionRemainingDelay;
    [HideInInspector]
    public AttackController AttackController { get; private set; }
    [HideInInspector]
    public Rigidbody rb { get; private set; }

    [Button("Activate")]
    public void Activate()
    {
      this.IsActive = true;
      this.navMeshAgent.enabled = true;
    }

    [Button("Deactivate")]
    public void DeActivate()
    {
      this.IsActive = false;
      this.navMeshAgent.isStopped = true;
    }

    [Button("Transition to new state")]
    public void TransitionTo(State nextState)
    {
      if (nextState != this.RemainState) {
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

    public void AddWayPoint(Vector3 pos)
    {
      this.WayPoints.Add(pos);
    }

    void Awake()
    {
      this.Init();
    }

    void OnEnable()
    {
      if (this.Damagable != null) {
        this.Damagable.OnDamaged += this.OnDamaged;
      }
    }

    void OnDisable()
    {
      if (this.Damagable != null) {
        this.Damagable.OnDamaged -= this.OnDamaged;
      }
    }

    void Update()
    {
      if (this.IsActive) {
        this.AttackController.Update();
        if (this.actionRemainingDelay > 0) {
          this.actionRemainingDelay -= Time.deltaTime;
        }
        else {
          this.CurrentState.PerformAction(this);
          this.actionRemainingDelay = this.CurrentState.ActionInterval;
        }
        if (this.transitionRemainingDelay > 0) {
          this.transitionRemainingDelay -= Time.deltaTime;
        }
        else {
          this.CurrentState.UpdateTransition(this);  
          this.transitionRemainingDelay = this.CurrentState.TransitionInterval;
        }
      }
    }

    void OnDrawGizmos()
    {
      if (this.CurrentState != null) {
        Gizmos.color = this.CurrentState.GizmoColor;
        Gizmos.DrawSphere(this.Eye.position, 0.3f);
      }
    }

    [Button("Set waypoints")]
    void SetWayPoints()
    {
      var wayPoints = Array.FindAll( 
        GameObject.FindGameObjectsWithTag("Debug"),
        gameObject => gameObject.name == "WayPoint");
      foreach (var wayPoint in wayPoints) {
        this.WayPoints.Add(wayPoint.transform.position); 
      }
    }

    [Button("Init")]
    void Init()
    {
      if (this.navMeshAgent == null) {
        this.navMeshAgent = this.GetComponent<NavMeshAgent>();
      }
      if (this.Damagable == null) {
        this.Damagable = this.GetComponent<BaseDamagable>();
      }
      if (this.rb == null) {
        this.rb = this.GetComponent<Rigidbody>();
      }
      if (this.Damagable != null) {
        this.Damagable.SetMaxHp(this.Data.Hp);
      }
      this.AttackController = new AttackController(this.Data, this.rb);
      this.navMeshAgent.speed = this.Data.Speed;
      this.navMeshAgent.acceleration = this.Data.Acceleration;
      this.navMeshAgent.stoppingDistance = this.Data.StoppingDistance;
      this.navMeshAgent.angularSpeed = this.Data.RotationSpeed;
      if (this.WayPoints == null) {
        this.WayPoints = new ();
      }
    }

    void OnDamaged()
    {
      if (this.CurrentState.Events != null &&
        this.CurrentState.Events.TryGetValue(
          nameof(DamagedTrigger),
          out Event monsterEvent
          )) {
        monsterEvent.OnEventOccur(this);
      }
    }
  }

}
