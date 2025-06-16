using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Unit
{
  [Serializable]
  public class RangeAttackController: AttackController
  {
    ProjectileData projectileData;
    Transform firePoint;

    public RangeAttackController(
      Stat stat,
      ProjectileData projectileData,
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
      var projectile = gameObject.GetComponent<BaseProjectile>(); 
      Debug.Log($"projectile: {projectile}");
      projectile.Data = this.projectileData;
      return (projectile);
    }
  }
}
