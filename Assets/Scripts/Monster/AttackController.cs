using System;
using UnityEngine;

namespace Monster
{
  [Serializable]
  public class AttackController
  {
    public Data Data { get; private set; }
    public bool IsAttackable => (this.remainingDelay <= 0);

    float remainingDelay;

    public AttackController(Data data)
    {
      this.Data = data;
      this.remainingDelay = this.Data.AttackDelay;
    }

    public void Attack(BaseDamagable damagable)
    {
      Debug.Log($"Attack: {damagable.gameObject.name} - {this.Data.AttackDamage}");
      damagable.TakeDamage(this.Data.AttackDamage);
      this.remainingDelay = this.Data.AttackDelay;
    }

    public void Update()
    {
      this.remainingDelay -= Time.deltaTime;
    }
  }

}
