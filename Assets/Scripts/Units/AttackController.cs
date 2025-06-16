using UnityEngine;

namespace Unit
{
  public class AttackController
  {
    public Stat Stat { get; private set; }
    public bool IsAttackable => (this.remainingDelay <= 0);
    protected float remainingDelay;
    protected Transform lastTarget;

    public AttackController(Stat stat)
    {
      this.Stat = stat;
      this.remainingDelay = this.Stat.AttackDelay;
    }

    public virtual void Attack(BaseDamagable damagable)
    {
    }

    public virtual void Update()
    {
      this.remainingDelay -= Time.deltaTime;
    }
  }
}
