using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

namespace Unit
{
  public class FieldUnit: BaseUnit, IChasable, IPatrolable, IAttackAble
  { 
    static WaitForSeconds WAIT_FOR_DIE = new WaitForSeconds(0.7f);
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
    public Rigidbody Rigidbody => this.rb;
    [ShowInInspector]
    public AttackController AttackController;
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

    [HideInInspector]
    public Transform Eye { get; private set; }
    Animator animator;

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

    protected override void Awake()
    {
      base.Awake();
      this.WayPoints = new ();
      if (this.navMeshAgent == null) {
        this.navMeshAgent = this.GetComponent<NavMeshAgent>();
      }
      if (this.rb == null) {
        this.rb = this.GetComponent<Rigidbody>();
      }
      if (this.animator == null) {
        this.animator = this.GetComponent<Animator>();
      }
      this.Eye = this.GetEyeTransform();
      if (this.IsActive) {
        this.Init();
      }
    }

    void OnEnable()
    {
      if (this.Damagable != null) {
        this.Damagable.OnDamaged += this.OnDamaged;
      }
    }

    protected override void OnDisable()
    {
      base.OnDisable();
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
      this.animator.SetFloat("MoveSpeed", this.navMeshAgent.velocity.magnitude);
    }

    void OnDrawGizmos()
    {
      if (this.StateController != null && 
        this.StateController.CurrentState != null) {
        Gizmos.color = this.StateController.CurrentState.GizmoColor;
        Gizmos.DrawSphere(this.Eye.position, 0.3f);
      }
    }

    Transform GetEyeTransform()
    {
      var eye = (Utils.RecursiveFindChild(this.transform, "Eye"));
      if (eye != null) {
        return (eye);
      }
      return (this.transform);
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
    public override void Init()
    {
      base.Init();
      if (this.Stat.IsMeleeMonster) {
        this.AttackController = new MeleeAttackController(this.Stat, this.transform);
      }
      else {
        this.AttackController = new RangeAttackController(
          stat: this.Stat,
          projectileStat: this.projectileStat,
          firePoint: this.Eye,
          attacker: this.transform
          );
      }
      this.navMeshAgent.speed = this.Stat.Speed;
      this.navMeshAgent.acceleration = this.Stat.Acceleration;
      this.navMeshAgent.stoppingDistance = this.Stat.StoppingDistance;
      this.navMeshAgent.angularSpeed = this.Stat.RotationSpeed;
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
      if (this.Damagable.Hp.Value.current > 0) {
        this.animator.SetTrigger("Hit");
      }
      else {
        this.StartCoroutine(this.DieRoutine());
      }
    }

    IEnumerator DieRoutine()
    {
      GameManager.Shared.Gold.Value += 5;
      this.IsActive = false;
      this.animator.SetTrigger("Die");
      yield return (WAIT_FOR_DIE);
      this.gameObject.SetActive(false);
    }

    public void Attack(BaseDamagable damagable)
    {
      this.AttackController.Attack(damagable);
      this.animator.SetTrigger("Attack");
    }
  }
}
