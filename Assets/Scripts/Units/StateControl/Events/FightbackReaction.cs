using System.Collections;
using System.Collections.Generic;
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
      var chasable = (IChasable)unit;
      if (unit.Damagable.Hp.Value.current > this.hpThreshold) {
        if (chasable.ChaseTarget != null) {
          return (true);
        }
        else {
          var attacker = unit.Damagable.LastAttacker.GetComponent<BaseDamagable>();
          if (attacker != null) {
            chasable.ChaseTarget = attacker;
            return (true);
          }
          else {
            return (false);
          }
        }
      }
      return (false);
    }
  }

}
