using UnityEngine;

namespace Unit
{
  public class MeleeAttackController: AttackController
  {

    public MeleeAttackController(Stat stat, Transform attacker): base(stat, attacker)
    {
      this.remainingDelay = this.Stat.AttackDelay;
    }

    public override void Attack(BaseDamagable damagable)
    {
      damagable.TakeDamage(this.Stat.AttackDamage, this.attacker);
    }
  }
}
