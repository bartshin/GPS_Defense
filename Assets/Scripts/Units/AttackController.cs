using UnityEngine;

namespace Unit
{
  public abstract class AttackController
  {
    public Stat Stat { get; protected set; }
    public bool IsAttackable => (this.remainingDelay <= 0);
    protected Transform attacker;
    protected float remainingDelay;

    public AttackController(Stat stat, Transform attacker)
    {
      this.Stat = stat;
      this.attacker = attacker;
      this.remainingDelay = this.Stat.AttackDelay;
    }

    public abstract void Attack(BaseDamagable damagable);

    public virtual void Update()
    {
      this.remainingDelay -= Time.deltaTime;
    }
  }
}
