using UnityEngine;

namespace Unit
{
  public interface IAttackAble
  {
    public bool IsAttackable { get; }
    public Vector3 AttackPosition { get; }
    public Vector3 AttackDirection { get; }
    public BaseDamagable Target { get; set; }
    public void Attack(BaseDamagable damagable);
  }
}
