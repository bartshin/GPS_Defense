using UnityEngine;
using Sirenix.OdinInspector;

namespace Monster
{
  [CreateAssetMenu(fileName = "NewMonsterData", menuName = "Data/Monster Data")]
  public class Data : _ScriptableObject
  {
    [HorizontalGroup("Prefab")] [LabelText("Monster")] [PreviewField(75)] [AssetsOnly] [AssetSelector(Paths = "Assets/Prefabs/Monsters")]
    public GameObject Prefab;
    [HorizontalGroup("Prefab")] [LabelText("Projectile")] [PreviewField(75)] [AssetsOnly] [AssetSelector(Paths = "Assets/Prefabs/Projectiles")]
    public GameObject ProjectilePrefab;
    [VerticalGroup("Basic")]
    public string Name;

    [BoxGroup("Health")] [Range(1f, 1000f)]
    public int Hp;

    [BoxGroup("Movement")] [Range(1f, 30f)]
    public float Speed;
    [BoxGroup("Movement")] [Range(1f, 30f)]
    public float Acceleration;
    [BoxGroup("Movement")] [Range(0.5f, 5f)]
    public float StoppingDistance;
    [BoxGroup("Movement")] [Range(10f, 50f)]
    public float RotationSpeed;

    [BoxGroup("Decision")] [Range(0.5f, 5f)]
    public float LookSpehreCastRadius;
    [BoxGroup("Decision")] [Range(5f, 50f)]
    public float LookRange;

    [BoxGroup("Attack")] [Range(0.5f, 150f)]
    public float ProjectileSpeed;
    [BoxGroup("Attack")] [Range(0.5f, 5f)]
    public float ProjectileLifeTime;
    [BoxGroup("Attack")] [Range(0, 30)]
    public int AttackDamage;
    [BoxGroup("Attack")] [Range(0.5f, 100f)]
    public float AttackRange;
    [BoxGroup("Attack")] [Range(0.1f, 3f)]
    public float AttackDelay;
  }

}
