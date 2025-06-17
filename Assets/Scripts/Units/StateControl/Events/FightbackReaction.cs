using UnityEngine;

namespace Unit
{
  [CreateAssetMenu (menuName = "Data/Unit/Reaction/Fightback")]
  public class FightbackReaction : Reaction
  {
    [SerializeField]
    float hpThreshold;

    public override bool React(BaseUnit unit)
    {
      bool isFightback = false;
      if (unit is IAttackAble attackAble) {
        if (unit.Damagable.Hp.Value.current > this.hpThreshold) {
          if (attackAble.Target != null) {
            isFightback = true;
          }
          else {
            var attacker = unit.Damagable.LastAttacker.GetComponent<BaseDamagable>();
            if (attacker != null) {
              attackAble.Target = attacker;
              isFightback = true;
            }
          }
          if (isFightback && attackAble.Target != null) {
            unit.transform.LookAt(attackAble.Target.transform);
          }
        }
      }
      return (isFightback);
    }
  }

}
