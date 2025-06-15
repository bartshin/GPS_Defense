using System;
using UnityEngine;

namespace Unit
{
  [Serializable]
  public class AttackController
  {
    public Data Data { get; private set; }
    public bool IsAttackable => (this.remainingDelay <= 0);
    Rigidbody rigidbody;
    float remainingDelay;
    Transform lastTarget;

    public AttackController(Data data, Rigidbody rigidbody)
    {
      this.Data = data;
      this.remainingDelay = this.Data.AttackDelay;
    }

    public void Attack(BaseDamagable damagable)
    {
      Debug.Log("Attack");
      damagable.TakeDamage(this.Data.AttackDamage);
      this.remainingDelay = this.Data.AttackDelay;
    }

    public void Update()
    {
      this.remainingDelay -= Time.deltaTime;
    }
  }
}
