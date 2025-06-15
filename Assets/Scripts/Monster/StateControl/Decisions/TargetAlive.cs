using UnityEngine;

namespace Monster
{
  [CreateAssetMenu(menuName = "Data/Monster/Decision/Target Alive")]
  public class TargetAliveDecision : Decision
  {
    public override bool Decide(Controller controller)
    {
      return (controller.ChaseTarget.IsAlive);
    }
  }

}
