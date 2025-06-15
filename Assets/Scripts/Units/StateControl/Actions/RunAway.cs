using UnityEngine;

namespace Unit
{
  [CreateAssetMenu (menuName = "Data/Unit/Action/Run away")]
  public class RunAwayAction : Action
  {
    [SerializeField]
    float runAwayDistance;

    public override void Act(Controller controller)
    {
      Vector3 dir;
      if (controller.Damagable.LastAttacker != null) {
        dir = - (controller.Damagable.LastAttacker.position - controller.transform.position).normalized; 
      }
      else {
        dir = (controller.Shelther - controller.transform.position).normalized;
      }
      controller.navMeshAgent.SetDestination(
        controller.transform.position + dir * this.runAwayDistance);
      controller.navMeshAgent.isStopped = false;
    }
  }
}

