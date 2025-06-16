using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Unit
{
  [Serializable]
  public class RangeAttackController: AttackController
  {
    ProjectileStat projectileStat;
    Transform firePoint;

    public RangeAttackController(
      Stat stat,
      ProjectileStat projectileStat,
      Transform firePoint, 
      Transform attacker): base(stat, attacker)
    { 
      this.projectileStat = projectileStat;
      this.firePoint = firePoint;
    }

    [Button ("Attack")]
    public override void Attack(BaseDamagable damagable)
    {
      this.remainingDelay = this.Stat.AttackDelay;
      var projectile = this.CreateProjectile();
      projectile.Target = damagable;
      projectile.gameObject.SetActive(true);
    }

    BaseProjectile CreateProjectile()
    {
      var gameObject = GameObject.Instantiate(this.projectileStat.Prefab);
      gameObject.transform.position = this.firePoint.position;
      var projectile = gameObject.GetComponent<BaseProjectile>(); 
      projectile.Data = this.projectileStat;
      var pooledObject = gameObject.GetComponent<SimplePooledObject>();
      pooledObject.LifeTime = this.projectileStat.LifeTime;
      projectile.FiredUnit = this.firePoint.gameObject;
      projectile.Damage = this.Stat.AttackDamage;
      return (projectile);
    }
  }
}
