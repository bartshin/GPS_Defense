using UnityEngine;
using Sirenix.OdinInspector;

namespace Unit
{
  public class Tower : BaseUnit, IAttackAble
  {
    Transform Muzzle; 
    [SerializeField] [Required(InfoMessageType.Error)]
    public ProjectileStat ProjectileStat;
    [ShowInInspector]
    public AttackController AttackController { get; private set; }
    Animator Animator;
    public bool IsAttackable => this.AttackController.IsAttackable;
    public Vector3 AttackPosition => this.Muzzle.position;
    public Vector3 AttackDirection => this.Muzzle.forward;
    public bool IsRotating => this.StateController.CurrentState == StateController.ROTATE_STATE;
    public bool IsFocusing => this.StateController.CurrentState == StateController.FOCUS_ATTACK_STATE;

    public BaseDamagable Target { get; set; }
    Transform rotatePart;

    public void Attack(BaseDamagable damagable)
    {
      this.AttackController.Attack(damagable);
      if (this.Animator != null) {
        this.Animator.SetTrigger("Attack");
      }
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

    public override void Init()
    {
      base.Init();
      this.AttackController = new RangeAttackController(
        stat: this.Stat,
        projectileStat: this.ProjectileStat,
        firePoint: this.Muzzle,
        attacker: this.transform);
    }

    protected override void Awake()
    {
      base.Awake();
      if (this.Animator == null) {
        this.Animator = this.GetComponent<Animator>();
      }
      if (this.Muzzle == null) {
        this.Muzzle = Utils.RecursiveFindChild(this.transform, "Muzzle");
      }
      if (this.rotatePart == null) {
        this.rotatePart = Utils.RecursiveFindChild(this.transform, "RotatePart");
        if (this.rotatePart == null) {
          this.rotatePart = this.transform;
        }
      }
      if (this.IsActive) {
        this.Init();
      }
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
        else if (this.IsFocusing && this.Target != null) {
          this.rotatePart.LookAt(
            new Vector3(
              this.Target.transform.position.x,
              this.transform.position.y,
              this.Target.transform.position.z
              )
            );
        }
      }
    }

    void Rotate()
    {
      this.rotatePart.Rotate(new Vector3(
          0, this.Stat.RotationSpeed * Time.deltaTime, 0
          )
        );
    }
  }
}
