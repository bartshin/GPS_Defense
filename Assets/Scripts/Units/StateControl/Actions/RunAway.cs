using UnityEngine;

namespace Unit
{
  [CreateAssetMenu (menuName = "Data/Unit/Action/Run away")]
  public class RunAwayAction : Action
  {
    [SerializeField]
    float runAwayDistance;

    public override void Act(BaseUnit unit)
    {
      if (unit.Damagable.LastAttacker != null) {
        var navMeshAgent = ((INavMeshMovable)unit).NavMeshAgent;
        var dir = - (unit.Damagable.LastAttacker.position - unit.transform.position).normalized; 
        navMeshAgent.SetDestination(
          unit.transform.position + dir * this.runAwayDistance);
        navMeshAgent.isStopped = false;
      }
    }
  }
}

