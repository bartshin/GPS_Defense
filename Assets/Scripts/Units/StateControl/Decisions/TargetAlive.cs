using UnityEngine;

namespace Unit
{
  [CreateAssetMenu(menuName = "Data/Unit/Decision/Target Alive")]
  public class TargetAliveDecision : Decision
  {
    public override bool Decide(BaseUnit unit)
    {
      var chasable = (IChasable)unit;
      var isAlive = chasable.ChaseTarget.IsAlive;
      if (!isAlive) {
        chasable.ChaseTarget = null;
      }
      return (isAlive);
    }
  }
}
