using UnityEngine;

namespace Unit
{
  [CreateAssetMenu (menuName = "Data/Unit/Decision/Target In AttackRange")]
  public class TargetInAttackRange : Decision
  {
    public override bool Decide(BaseUnit unit)
    {
      if (unit is IAttackAble attackAble &&
        attackAble.Target != null) {
        var dist = Vector3.Distance(
          attackAble.AttackPosition,
          attackAble.Target.transform.position);
        return (dist < unit.Stat.AttackRange);
      }
      return (false);
    }
  }
}
