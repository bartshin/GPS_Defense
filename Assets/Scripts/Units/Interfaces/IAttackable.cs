using UnityEngine;

namespace Unit
{
  public interface IAttackAble
  {
    public bool IsAttackable { get; }
    public Vector3 AttackPosition { get; }
    public Vector3 AttackDirection { get; }
    public void Attack(BaseDamagable damagable);
  }
}
