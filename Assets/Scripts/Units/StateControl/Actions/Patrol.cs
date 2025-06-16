using UnityEngine;

namespace Unit
{
  [CreateAssetMenu(menuName = "Data/Unit/Action/Patrol")]
  public class PatrolAction : Action
  {
    public override void Act(BaseUnit unit)
    {
      this.Patrol(unit);
    }

    void Patrol(BaseUnit unit)
    {
      var patrolAble = (IPatrolable)unit;
      var navMeshAgent = patrolAble.NavMeshAgent;
      navMeshAgent.destination = patrolAble.WayPoints[patrolAble.NextWayPoint];
      navMeshAgent.isStopped = false;

      if (!navMeshAgent.pathPending &&
        (navMeshAgent.remainingDistance <
         navMeshAgent.stoppingDistance)) {
       patrolAble.NextWayPoint = (patrolAble.NextWayPoint + 1) % patrolAble.WayPoints.Count;
      }
    }
  }
}
