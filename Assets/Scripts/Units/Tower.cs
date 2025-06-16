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
    public AttackController AttackController { get; private set; }
    public bool IsAttackable => this.AttackController.IsAttackable;
    public Vector3 AttackPosition => this.attackPoint.position;
    public Vector3 AttackDirection => this.attackPoint.forward;
    float stopRotatingDuration = 0;
    Vector3 rotationVector;

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
      this.AttackController = new AttackController(this.Stat);
      this.rotationVector = new Vector3(0, this.Stat.RotationSpeed, 0);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      if (this.IsActive) {
        if (this.stopRotatingDuration <= 0) {
          this.Rotate();
        }
        else {
          this.stopRotatingDuration -= Time.deltaTime;
        }
      }
    }

    void Rotate()
    {
      this.transform.Rotate(this.rotationVector);
    }

    public void Attack(BaseDamagable damagable)
    {
      this.AttackController.Attack(damagable);
    }
  }
}
