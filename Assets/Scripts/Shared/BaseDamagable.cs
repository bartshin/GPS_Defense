using System;
using UnityEngine;
using Architecture;
using Sirenix.OdinInspector;

public class BaseDamagable : MonoBehaviour, IDamagable
{
  [ShowInInspector]
  public ObservableValue<(int current, int max)> Hp { get; private set; } = new ();
  public int LastDamage { get; private set; }
  public Transform LastAttacker { get; private set; }
  public bool IsAlive => this.Hp.Value.current > 0;
  public Vector3 Position => this.transform.position;
  public Action OnDamaged;

  [Button ("Set max hp")]
  public void SetMaxHp(int hp)
  {
    this.Hp.Value = (hp, hp);
  }

  [Button ("TakeDamage")]
  public int TakeDamage(int attackDamage)
  {
    var (current, max) = this.Hp.Value;
    var damageTaken = Math.Min(current, attackDamage);
    this.LastDamage = damageTaken;
    this.Hp.Value = (current - damageTaken, max);
    if (this.OnDamaged != null) {
      this.OnDamaged.Invoke();  
    }
    return (damageTaken);
  }

  [Button ("TakeDamage from")]
  int IDamagable.TakeDamage(int attackDamage, Transform attacker)
  {
    this.LastAttacker = attacker;
    return (this.TakeDamage(attackDamage));
  }
}
