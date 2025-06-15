using UnityEngine;

namespace Unit
{
  [CreateAssetMenu(menuName = "Data/Unit/Decision/Target Alive")]
  public class TargetAliveDecision : Decision
  {
    public override bool Decide(Controller controller)
    {
      var isAlive = (controller.ChaseTarget.IsAlive);
      if (!isAlive) {
        controller.ChaseTarget = null;
      }
      return (isAlive);
    }
  }
}
