using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu (menuName = "Data/Projectile")]
public class ProjectileStat : _ScriptableObject
{
  [BoxGroup("Prefab")] [PreviewField(75)] [AssetsOnly] 
  public GameObject Prefab;
  [BoxGroup("Sound")] [PreviewField(75)] [AssetsOnly] 
  public AudioClip FireSound;
  [BoxGroup("Sound")] [PreviewField(75)] [AssetsOnly] 
  public AudioClip ExplosionSound;
  [SerializeField]
  public LayerMask TargetLayer;
  [SerializeField]
  public float Speed;
  [SerializeField]
  public float LifeTime;
}
