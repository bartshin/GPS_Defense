using UnityEngine;

public interface IDamagable
{
  public int TakeDamage(int attackDamage);
  public bool IsAlive { get; }
  public Vector3 Position { get; }

  public virtual int TakeDamage(int attackDamage, Transform attacker)
  {
    return (this.TakeDamage(attackDamage));
  }

  public virtual int TakeDamage(
    int attackDamage,
    Transform attacker,
    Vector3 attackedPosition) {
    return (this.TakeDamage(attackDamage, attacker));
  }
}
