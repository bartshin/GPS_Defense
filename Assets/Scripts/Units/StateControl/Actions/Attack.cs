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
      var attackable = (IAttackAble)unit;
     #if UNITY_EDITOR
      Debug.DrawRay(
        start: new Vector3(
          attackable.AttackPosition.x,
          unit.transform.position.y,
          attackable.AttackPosition.z
          ),
        dir: attackable.AttackDirection * unit.Stat.AttackRange,
        this.gizmoColor, 0.3f);
      #endif
      if (attackable.IsAttackable) {
        this.Attack(unit);
      }
    }

    void Attack(BaseUnit unit)
    {
      var attackable = (IAttackAble)unit;
      if (
        Physics.SphereCast(
          new Vector3(
            attackable.AttackPosition.x,
            unit.transform.position.y,
            attackable.AttackPosition.z
            ),
          unit.Stat.LookSpehreCastRadius,
          attackable.AttackDirection,
          out RaycastHit hitInfo,
          unit.Stat.AttackRange)
        ) {
        var hitLayer = (1 << hitInfo.collider.gameObject.layer);
        if ((hitLayer & this.targetLayer.value) == 0) {
          return;
        }
        var damagable = UnitManager.Shared.GetDamagableFrom(hitInfo.collider.gameObject);
        if (damagable != null && damagable.IsAlive) {
          attackable.Attack(damagable);    
        }
      }
    }
  }
}
