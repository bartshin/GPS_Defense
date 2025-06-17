using System.Collections.Generic;
using UnityEngine;
using Architecture;

public class ProjectilePool : SingletonBehaviour<ProjectilePool>
{
  [SerializeField]
  static int NUMBER_OF_PROJECTILES = 30;
  Dictionary<GameObject, MonoBehaviourPool<BaseProjectile>> pool;


  override protected void Awake()
  {
    base.Awake();
    this.pool = new (); 
  }
    
  public BaseProjectile GetProjectile(ProjectileStat stat)
  {
    return (this.pool[stat.Prefab].Get());
  }

  public void SetProjectile(ProjectileStat stat)
  {
    if (!this.pool.ContainsKey(stat.Prefab)) {
      this.pool.Add(stat.Prefab, 
        new MonoBehaviourPool<BaseProjectile>(
          poolSize: NUMBER_OF_PROJECTILES,
          prefab: stat.Prefab,
          initPooledObject: (Projectile) => Projectile.Data = stat
          ));
    }     
  }
}
