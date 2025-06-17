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
      var targetPosition = chasable.ChaseTarget.transform.position;
      var stopDist = unit.Stat.StoppingDistance;
      var navMeshAgent = chasable.NavMeshAgent;
      if (navMeshAgent.hasPath &&
        navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance
        ){
        var dist = Vector3.Distance(
          unit.transform.position,
          targetPosition
          );
        if (dist < stopDist) {
            var dir = (unit.transform.position - targetPosition).normalized;
            chasable.Rigidbody.AddForce(
                dir * stopDist * 2f * Time.deltaTime,
                ForceMode.VelocityChange
              );
          unit.transform.LookAt(targetPosition);
          navMeshAgent.isStopped = true;
          return;
        }
      }
      if (Physics.Raycast(
          unit.transform.position,
          unit.transform.forward,
          out RaycastHit hitInfo,
          unit.Stat.LookRange) &&
        hitInfo.transform == chasable.ChaseTarget.transform &&
        hitInfo.distance < stopDist) {
        navMeshAgent.isStopped = true;
      }
      else {
        navMeshAgent.SetDestination(chasable.ChaseTarget.transform.position);
        navMeshAgent.isStopped = false;
      }
    }
  }
}
