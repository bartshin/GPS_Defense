using UnityEngine;

namespace Unit
{
  [CreateAssetMenu(menuName = "Data/Unit/Action/Chase")]
  public class ChaseAction : Action
  {
    public override void Act(Controller controller)
    {
      this.Chase(controller);
    }

    void Chase(Controller controller)
    {
      if (controller.navMeshAgent.hasPath &&
        controller.navMeshAgent.remainingDistance < controller.navMeshAgent.stoppingDistance
        ){
        var dist = Vector3.Distance(
          controller.transform.position,
          controller.ChaseTarget.transform.position
          );
        if (dist < controller.Data.StoppingDistance) {
          controller.transform.LookAt(controller.ChaseTarget.transform);
          controller.navMeshAgent.isStopped = true;
          return;
        }
      }
      controller.navMeshAgent.SetDestination(controller.ChaseTarget.transform.position);
      controller.navMeshAgent.isStopped = false;
    }
  }
}
