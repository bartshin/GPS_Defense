using UnityEngine;

namespace Monster
{
  [CreateAssetMenu(menuName = "Data/Monster/Action/Chase")]
  public class ChaseAction : Action
  {
    public override void Act(Controller controller)
    {
      this.Chase(controller);
    }

    void Chase(Controller controller)
    {
      controller.navMeshAgent.SetDestination(controller.ChaseTarget.transform.position);
      controller.navMeshAgent.isStopped = false;
    }
  }

}
