using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu (menuName = "Data/Projectile")]
public class ProjectileStat : _ScriptableObject
{
  [BoxGroup("Prefab")] [PreviewField(75)] [AssetsOnly] 
  public GameObject Prefab;
  [SerializeField]
  public LayerMask TargetLayer;
  [SerializeField]
  public float Speed;
  [SerializeField]
  public float LifeTime;
}
