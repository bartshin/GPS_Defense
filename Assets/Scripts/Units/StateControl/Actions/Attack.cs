using UnityEngine;

namespace Unit
{
  [CreateAssetMenu(menuName = "Data/Unit/Action/Attack")]
  public class AttackAction : Action
  {
    [SerializeField]
    Color gizmoColor;
    [SerializeField]
    LayerMask targetLayer;
    public override void Act(Controller controller)
    {
     #if UNITY_EDITOR
      Debug.DrawRay(
        start: controller.Eye.position,
        dir: controller.Eye.forward * controller.Data.AttackRange,
        this.gizmoColor, 0.3f);
      #endif
      if (controller.AttackController.IsAttackable) {
        this.Attack(controller);
      }
    }

    void Attack(Controller controller)
    {
      if (
        Physics.SphereCast(
          controller.Eye.position,
          controller.Data.LookSpehreCastRadius,
          controller.Eye.forward,
          out RaycastHit hitInfo,
          controller.Data.AttackRange)
        ) {
        var hitLayer = (1 << hitInfo.collider.gameObject.layer);
        if ((hitLayer & this.targetLayer.value) == 0) {
          return;
        }
        //FIXME: remove GetComponent 
        var damagable = hitInfo.collider.gameObject.GetComponent<BaseDamagable>();
        if (damagable != null && damagable.IsAlive) {
          controller.AttackController.Attack(damagable);    
        }
      }
    }
  }

}
