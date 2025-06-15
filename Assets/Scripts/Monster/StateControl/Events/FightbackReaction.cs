using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Monster
{
  [CreateAssetMenu (menuName = "Data/Monster/Reaction/Fightback")]
  public class FightbackReaction : Reaction
  {
    [SerializeField]
    float hpThreshold;

    public override bool React(Controller controller)
    {
      if (controller.Damagable.Hp.Value.current > this.hpThreshold) {
        if (controller.ChaseTarget != null) {
          return (true);
        }
        else {
          var attacker = controller.Damagable.LastAttacker.GetComponent<BaseDamagable>();
          if (attacker != null) {
            controller.ChaseTarget = attacker;
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
