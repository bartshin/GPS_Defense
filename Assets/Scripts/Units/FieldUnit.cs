using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

namespace Unit
{
  public class FieldUnit: BaseUnit, IChasable, IPatrolable, IAttackAble
  { 
    [SerializeField] [Required(InfoMessageType.Error)] 
    public Transform Eye { get; private set; }
    public NavMeshAgent NavMeshAgent => this.navMeshAgent;
    [HideInInspector]
    public NavMeshAgent navMeshAgent { get; private set; }
    [ShowInInspector]
    public List<Vector3> WayPoints { get; private set; }
    [SerializeField]
    public Vector3 Shelther { get; private set; }
    public int NextWayPoint { 
      get => this.nextWayPoint;
      set => this.nextWayPoint = value;
    }
    [HideInInspector]
    public int nextWayPoint;
    public BaseDamagable ChaseTarget { get; set; }
    [ShowInInspector]
    public AttackController AttackController { get; private set; }
    [HideInInspector]
    public Rigidbody rb { get; private set; }

    public bool IsAttackable => this.AttackController.IsAttackable;
    public Vector3 AttackPosition => this.Eye.position;
    public Vector3 AttackDirection => this.Eye.forward;
    public BaseDamagable Target 
    {
      get => this.ChaseTarget;
      set => this.ChaseTarget = value;
    }

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

    protected override void Update()
    {
      base.Update();
      if (this.IsActive) {
        this.AttackController.Update();
      }
    }

    void OnDrawGizmos()
    {
      if (this.StateController != null && 
        this.StateController.CurrentState != null) {
        Gizmos.color = this.StateController.CurrentState.GizmoColor;
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
    protected override void Init()
    {
      base.Init();
      if (this.navMeshAgent == null) {
        this.navMeshAgent = this.GetComponent<NavMeshAgent>();
      }
      if (this.rb == null) {
        this.rb = this.GetComponent<Rigidbody>();
      }
      this.AttackController = new AttackController(this.Stat);
      this.navMeshAgent.speed = this.Stat.Speed;
      this.navMeshAgent.acceleration = this.Stat.Acceleration;
      this.navMeshAgent.stoppingDistance = this.Stat.StoppingDistance;
      this.navMeshAgent.angularSpeed = this.Stat.RotationSpeed;
      if (this.WayPoints == null) {
        this.WayPoints = new ();
      }
    }

    void OnDamaged()
    {
      if (this.StateController.CurrentState.Events != null &&
        this.StateController.CurrentState.Events.TryGetValue(
          nameof(DamagedTrigger),
          out Event unitEvent
          )) {
        unitEvent.OnEventOccur(this);
      }
    }

        public void Attack(BaseDamagable damagable)
        {
            throw new NotImplementedException();
        }
    }
}
