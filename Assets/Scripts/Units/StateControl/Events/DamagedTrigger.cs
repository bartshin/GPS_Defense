using UnityEngine;

namespace Unit
{
  [CreateAssetMenu(menuName = "Data/Unit/Trigger/Damaged")]
  public class DamagedTrigger : Trigger
  {
    [SerializeField]
    int agroThreshold;

    public override bool IsTriggerToReact(BaseUnit unit)
    {
      if (unit.Damagable.LastDamage > this.agroThreshold) {
        return (true);
      }
      return (false);
    }
  }

}
