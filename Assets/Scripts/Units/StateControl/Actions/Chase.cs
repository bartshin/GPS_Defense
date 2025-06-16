using UnityEngine;

namespace Unit
{
  [CreateAssetMenu(menuName = "Data/Unit/Action/Chase")]
  public class ChaseAction : Action
  {
    public override void Act(BaseUnit unit)
    {
      this.Chase(unit);
    }

    void Chase(BaseUnit unit)
    {
      var chasable = (IChasable)unit;
      var navMeshAgent = chasable.NavMeshAgent;
      if (navMeshAgent.hasPath &&
        navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance
        ){
        var dist = Vector3.Distance(
          unit.transform.position,
          chasable.ChaseTarget.transform.position
          );
        if (dist < unit.Stat.StoppingDistance) {
          unit.transform.LookAt(chasable.ChaseTarget.transform);
          navMeshAgent.isStopped = true;
          return;
        }
      }
      navMeshAgent.SetDestination(chasable.ChaseTarget.transform.position);
      navMeshAgent.isStopped = false;
    }
  }
}
