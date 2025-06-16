using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Unit
{
  public class Tower : BaseUnit, IAttackAble
  {
    [SerializeField] [Required(InfoMessageType.Error)]
    Transform attackPoint; 
    [SerializeField]
    State rotateState;
    [SerializeField] [Required(InfoMessageType.Error)]
    ProjectileData projectileData;
    [ShowInInspector]
    public AttackController AttackController { get; private set; }
    public bool IsAttackable => this.AttackController.IsAttackable;
    public Vector3 AttackPosition => this.attackPoint.position;
    public Vector3 AttackDirection => this.attackPoint.forward;
    public bool IsRotating => this.StateController.CurrentState == this.rotateState;

    void Awake()
    {
      this.Init();
    }

    [Button ("Activate")]
    public void Activate()
    {
      this.IsActive = true;
    }

    [Button ("Deactivate")]
    public void DeActivate()
    {
      this.IsActive = false;
    }

    protected override void Init()
    {
      base.Init();
      this.AttackController = new RangeAttackController(
        stat: this.Stat,
        projectileData: this.projectileData,
        firePoint: this.attackPoint);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    protected override void Update()
    {
      base.Update();
      if (this.IsActive) {
        this.AttackController.Update();
        if (this.IsRotating) {
          this.Rotate();
        }
      }
    }

    void Rotate()
    {
      this.transform.Rotate(new Vector3(
          0, this.Stat.RotationSpeed * Time.deltaTime, 0
          )
        );
    }

    public void Attack(BaseDamagable damagable)
    {
      this.AttackController.Attack(damagable);
    }
  }
}
