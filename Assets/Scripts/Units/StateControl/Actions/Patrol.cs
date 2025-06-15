using UnityEngine;

namespace Unit
{
  [CreateAssetMenu(menuName = "Data/Unit/Action/Patrol")]
  public class PatrolAction : Action
  {
    public override void Act(Controller controller)
    {
      this.Patrol(controller);
    }

    void Patrol(Controller controller)
    {
      controller.navMeshAgent.destination = controller.WayPoints[controller.nextWayPoint];
      controller.navMeshAgent.isStopped = false;

      if (!controller.navMeshAgent.pathPending &&
        (controller.navMeshAgent.remainingDistance <
         controller.navMeshAgent.stoppingDistance)) {
        controller.nextWayPoint = (controller.nextWayPoint + 1) % controller.WayPoints.Count;
      }
    }
  }

}
