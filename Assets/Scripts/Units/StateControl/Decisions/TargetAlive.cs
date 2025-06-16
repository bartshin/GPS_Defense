using UnityEngine;

namespace Unit
{
  [CreateAssetMenu(menuName = "Data/Unit/Decision/Target Alive")]
  public class TargetAliveDecision : Decision
  {
    public override bool Decide(BaseUnit unit)
    {
      bool isAlive = false;
      if (unit is IAttackAble attackAble) {
        isAlive = attackAble.Target.IsAlive;
        if (!isAlive) {
          attackAble.Target = null;
        }
      }
      return (isAlive);
    }
  }
}
