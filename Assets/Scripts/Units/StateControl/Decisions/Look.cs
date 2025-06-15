using UnityEngine;

namespace Unit
{
  [CreateAssetMenu(menuName = "Data/Unit/Decision/Look")]
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
        color: this.gizmoColor, 0.3f);
#endif
      return (this.IsTargetFound(controller)); 
    }

    bool IsTargetFound(Controller controller)
    {
      if (
        Physics.SphereCast(
        controller.Eye.position,
        controller.Data.LookSpehreCastRadius,
        controller.Eye.forward,
        out RaycastHit hitInfo,
        controller.Data.LookRange)
        ) {
        Debug.Log("look hit");
        //FIXME: remove GetComponent 
        var hitLayer = (1 << hitInfo.collider.gameObject.layer);
        if ((hitLayer & this.targetLayer.value) == 0) {
          return (false);
        }
        var damagable = hitInfo.collider.GetComponent<BaseDamagable>();
        if (damagable != null) {
          controller.ChaseTarget = damagable;
          return (true);
        }
      }
      return (false);
    }
  }

}
