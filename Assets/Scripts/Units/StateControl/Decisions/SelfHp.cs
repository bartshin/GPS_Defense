using UnityEngine;

namespace Unit
{
  [CreateAssetMenu (menuName = "Data/Unit/Decision/Self Hp")]
  public class SelfHpDecision : Decision
  {
    [SerializeField] [Range(0f, 1f)]
    float hpPercentageThreshold;

    public override bool Decide(BaseUnit unit)
    {
      var (current, max) = unit.Damagable.Hp.Value;
      return (this.hpPercentageThreshold > ((float)current / (float)max));
    }
  }
}
