using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Unit
{
  [Serializable]
  public class RangeAttackController: AttackController
  {
    ProjectileStat projectileData;
    Transform firePoint;

    public RangeAttackController(
      Stat stat,
      ProjectileStat projectileData,
      Transform firePoint): base(stat)
    { 
      this.projectileData = projectileData;
      this.firePoint = firePoint;
    }

    [Button ("Attack")]
    public override void Attack(BaseDamagable damagable)
    {
      this.remainingDelay = this.Stat.AttackDelay;
      var projectile = this.CreateProjectile();
      projectile.Target = damagable;
      projectile.FiredUnit = this.firePoint.gameObject;
      projectile.gameObject.SetActive(true);
    }

    BaseProjectile CreateProjectile()
    {
      var gameObject = GameObject.Instantiate(this.projectileData.Prefab);
      gameObject.transform.position = this.firePoint.position;
      var projectile = gameObject.GetComponent<BaseProjectile>(); 
      projectile.Data = this.projectileData;
      var pooledObject = gameObject.GetComponent<SimplePooledObject>();
      pooledObject.LifeTime = this.projectileData.LifeTime;
      return (projectile);
    }
  }
}
