using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

public class MonsterController : _MonoBehaviour
{
  [SerializeField] [Required(InfoMessageType.Error)]
  public Transform Eye { get; private set; }
  [SerializeField]
  public MonsterState State { get; private set; }
  [SerializeField] [Required(InfoMessageType.Warning)]
  MonsterState RemainState;
  [HideInInspector]
  public NavMeshAgent navMeshAgent { get; private set; }
  [SerializeField]
  public MonsterData Data { get; private set; }
  [ShowInInspector]
  public List<Vector3> WayPoints { get; private set; }
  [SerializeField]
  public bool IsActive { get; private set; }
  [HideInInspector]
  public int nextWayPoint;
  public Transform ChaseTarget;

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
    this.navMeshAgent.enabled = false;
  }
  
  [Button("Transition to new state")]
  public void TransitionTo(MonsterState nextState)
  {
    if (nextState != this.RemainState) {
      this.State = nextState; 
    }
  }

  public void AddWayPoint(Vector3 pos)
  {
    this.WayPoints.Add(pos);
  }
  
  void Awake()
  {
    this.Init();
  }

  void Update()
  {
    if (!this.IsActive) {
      return ;
    }
    this.State.UpdateState(this);  
  }

  void OnDrawGizmos()
  {
    if (this.State != null) {
      Gizmos.color = this.State.GizmoColor;
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
    this.navMeshAgent.speed = this.Data.Speed;
    this.navMeshAgent.acceleration = this.Data.Acceleration;
    this.navMeshAgent.stoppingDistance = this.Data.StoppingDistance;
    this.navMeshAgent.angularSpeed = this.Data.RotationSpeed;
  
    if (this.WayPoints == null) {
      this.WayPoints = new ();
    }
  }
}
