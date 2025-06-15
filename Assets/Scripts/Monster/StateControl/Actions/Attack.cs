using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monster
{
  [CreateAssetMenu(menuName = "Data/Monster/Action/Attack")]
  public class AttackAction : Action
  {
    [SerializeField]
    Color gizmoColor;
    [SerializeField]
    LayerMask targetLayer;
    public override void Act(Controller controller)
    {
      if (controller.AttackController.IsAttackable) {
        this.Attack(controller);
      }
    }

    void Attack(Controller controller)
    {
#if UNITY_EDITOR
      Debug.DrawRay(
        controller.Eye.transform.position,
        controller.Eye.forward * controller.Data.AttackRange,
        this.gizmoColor,
        0.5f
        );
#endif
      bool isHit = Physics.SphereCast(
        origin: controller.Eye.transform.position,
        radius: controller.Data.LookSpehreCastRadius,
        direction: controller.Eye.forward,
        out RaycastHit hitInfo,
        maxDistance:  controller.Data.AttackRange
        );
      if (isHit && (
          (1 << hitInfo.collider.gameObject.layer) & this.targetLayer.value
          ) != 0) {
        //FIXME: remove GetComponent 
        var damagable = hitInfo.collider.gameObject.GetComponent<BaseDamagable>();
        if (damagable != null && damagable.IsAlive) {
          controller.AttackController.Attack(damagable);    
        }
      }
    }
  }

}
