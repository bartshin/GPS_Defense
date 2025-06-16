using UnityEngine;

namespace Unit
{
  [CreateAssetMenu (menuName = "Data/Unit/Decision/Target In Look Range")]
  public class TargetInLookRange : Decision
  {
    public override bool Decide(BaseUnit unit)
    {
      if (unit is IAttackAble attackAble &&
        attackAble.Target != null) {
        var dist = Vector3.Distance(
          unit.transform.position,
          attackAble.Target.Position
          );
        return (dist < unit.Stat.LookRange);
      }
      return (false);
    }
  }
}
