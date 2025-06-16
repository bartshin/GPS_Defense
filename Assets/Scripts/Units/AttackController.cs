using System;
using UnityEngine;

namespace Unit
{
  [Serializable]
  public class AttackController
  {
    public Stat Data { get; private set; }
    public bool IsAttackable => (this.remainingDelay <= 0);
    float remainingDelay;
    Transform lastTarget;

    public AttackController(Stat data)
    {
      this.Data = data;
      this.remainingDelay = this.Data.AttackDelay;
    }

    public void Attack(BaseDamagable damagable)
    {
      damagable.TakeDamage(this.Data.AttackDamage);
      this.remainingDelay = this.Data.AttackDelay;
    }

    public void Update()
    {
      this.remainingDelay -= Time.deltaTime;
    }
  }
}
