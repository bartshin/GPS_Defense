using UnityEngine;

namespace Monster
{
  [CreateAssetMenu(menuName = "Data/Monster/Decision/Look")]
  public class LookDecision : Decision
  {
    [SerializeField]
    Color gizmoColor;
    [SerializeField]
    LayerMask targetLayer;

    public override bool Decide(Controller controller)
    {
#if UNITY_EDITOR 
      Debug.DrawRay(
        start: controller.Eye.position,
        dir: controller.Eye.forward.normalized * controller.Data.LookRange, 
        color: this.gizmoColor);
#endif
      return (this.IsTargetFound(controller)); 
    }

    bool IsTargetFound(Controller controller)
    {
      bool isHit = Physics.SphereCast(
        controller.Eye.position,
        controller.Data.LookSpehreCastRadius,
        controller.Eye.forward,
        out RaycastHit hit,
        controller.Data.LookRange
        );
      if (isHit && ((1 << hit.collider.gameObject.layer) 
          & this.targetLayer.value) != 0) {
        //FIXME: remove GetComponent 
        var damagable = hit.collider.GetComponent<BaseDamagable>();
        if (damagable != null) {
          controller.ChaseTarget = damagable;
          return (true);
        }
      }
      return (false);
    }
  }

}
