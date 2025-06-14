using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Monster Action/Patrol")]
public class MonsterPatrolAction : MonsterAction
{
  public override void Act(MonsterController controller)
  {
    this.Patrol(controller);
  }
  
  void Patrol(MonsterController controller)
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
