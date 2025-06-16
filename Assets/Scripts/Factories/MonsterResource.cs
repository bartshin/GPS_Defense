using UnityEngine;
using Sirenix.OdinInspector;

namespace Unit
{
  [CreateAssetMenu (menuName = "Data/Factory/Monster Resource") ]
  public class MonsterResource : _ScriptableObject
  {
    [BoxGroup("Prefab")] [Required(InfoMessageType.Error)] [PreviewField(75)] [AssetsOnly] 
    public GameObject Prefab;
    [SerializeField] [Required(InfoMessageType.Error)]
    public Stat MonsterStat;  
    [SerializeField] [Required(InfoMessageType.Error)]
    public State DefaultState;
    [SerializeField] [Range(0f, 1f)] [Required(InfoMessageType.Error)]
    public float SpawnChance;
  }
}
