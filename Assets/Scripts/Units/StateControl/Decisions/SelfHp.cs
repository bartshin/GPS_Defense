using UnityEngine;
using Sirenix.OdinInspector;

namespace Unit
{
  [CreateAssetMenu (menuName = "Data/Unit/Decision/Self Hp")]
  public class SelfHpDecision : Decision
  {
    [SerializeField] [Range(0f, 1f)]
    float hpPercentageThreshold;

    public override bool Decide(Controller controller)
    {
      var (current, max) = controller.Damagable.Hp.Value;
      return (this.hpPercentageThreshold > ((float)current / (float)max));
    }
  }
}
