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

    public override bool Decide(BaseUnit unit)
    {
#if UNITY_EDITOR 
      var attackable = (IAttackAble)unit;
      Debug.DrawRay(
        start: attackable.AttackPosition,
        dir: attackable.AttackDirection* unit.Stat.LookRange, 
        color: this.gizmoColor, 0.3f);
#endif
      return (this.IsTargetFound(unit)); 
    }

    bool IsTargetFound(BaseUnit unit)
    {
      var attackable = (IAttackAble)unit;
      if (
        Physics.SphereCast(
        attackable.AttackPosition,
        unit.Stat.LookSpehreCastRadius,
        attackable.AttackDirection,
        out RaycastHit hitInfo,
        unit.Stat.LookRange)
        ) {
        var hitLayer = (1 << hitInfo.collider.gameObject.layer);
        if ((hitLayer & this.targetLayer.value) == 0) {
          return (false);
        }
        var damagable = UnitManager.Shared.GetDamagableFrom(hitInfo.collider.gameObject);
        if (damagable != null) {
          if (unit is IChasable chasable) {
            chasable.ChaseTarget = damagable;
          }
          return (true);
        }
      }
      return (false);
    }
  }

}
