using UnityEngine;
using Sirenix.OdinInspector;

namespace Unit
{
  [CreateAssetMenu (menuName = "Data/Factory/Tower Resource")]
  public class TowerResource : _ScriptableObject
  {
    [BoxGroup("Prefab")] [Required(InfoMessageType.Error)] [PreviewField(75)] [AssetsOnly] 
    public GameObject Prefab;
    [SerializeField] [Required(InfoMessageType.Error)]
    public Stat TowerStat;
    [SerializeField] [Required(InfoMessageType.Error)]
    public ProjectileStat Projectile;
    [SerializeField] [Required(InfoMessageType.Error)]
    public State DefaultState;
    [SerializeField] [Required(InfoMessageType.Warning)]
    public float TowerHeight;
  }
}
