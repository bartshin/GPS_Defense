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

    public override void Act(BaseUnit unit)
    {
     #if UNITY_EDITOR
      var attackable = (IAttackAble)unit;
      var fieldUnit = (FieldUnit)unit;
      Debug.DrawRay(
        start: attackable.AttackPosition,
        dir: attackable.AttackDirection * fieldUnit.Stat.AttackRange,
        this.gizmoColor, 0.3f);
      #endif
      if (attackable.IsAttackable) {
        this.Attack(unit);
      }
    }

    void Attack(BaseUnit unit)
    {
      var attackAble = (IAttackAble)unit;
      if (
        Physics.SphereCast(
          attackAble.AttackPosition,
          unit.Stat.LookSpehreCastRadius,
          attackAble.AttackDirection,
          out RaycastHit hitInfo,
          unit.Stat.AttackRange)
        ) {
        var hitLayer = (1 << hitInfo.collider.gameObject.layer);
        if ((hitLayer & this.targetLayer.value) == 0) {
          return;
        }
        var damagable = UnitManager.Shared.GetDamagableFrom(hitInfo.collider.gameObject);
        if (damagable != null && damagable.IsAlive) {
          attackAble.Attack(damagable);    
        }
      }
    }
  }

}
