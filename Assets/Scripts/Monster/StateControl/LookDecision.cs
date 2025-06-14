using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Monster Decision/Look")]
public class MonsterLookDecision : MonsterDecision
{
  [SerializeField]
  Color gizmoColor = Color.yellow;
  [SerializeField]
  LayerMask targetLayer;

  public override bool Decide(MonsterController controller)
  {
#if UNITY_EDITOR 
    Debug.DrawRay(
      start: controller.Eye.position,
      dir: controller.Eye.forward.normalized * controller.Data.LookRange, 
      color: this.gizmoColor);
#endif
    return (this.IsAbleToLook(controller)); 
  }

  bool IsAbleToLook(MonsterController controller)
  {
    if (Physics.SphereCast(
        controller.Eye.position,
        controller.Data.LookSpehreCastRadius,
        controller.Eye.forward,
        out RaycastHit hit,
        controller.Data.LookRange
        )) {
      if (((1 << hit.collider.gameObject.layer) 
        & this.targetLayer.value) != 0) {
        controller.ChaseTarget = hit.transform;
        return (true);
      }
      return (false);
    }
    return (false);
  }
}
