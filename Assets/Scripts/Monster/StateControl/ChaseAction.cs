using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Monster Action/Chase")]
public class MonsterChaseAction : MonsterAction
{
  public override void Act(MonsterController controller)
  {
    this.Chase(controller);
  }

  void Chase(MonsterController controller)
  {
    controller.navMeshAgent.SetDestination(controller.ChaseTarget.position);
    controller.navMeshAgent.isStopped = false;
  }
}
