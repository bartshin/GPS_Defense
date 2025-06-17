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
      ProjectilePool.Shared.SetProjectile(projectileStat);
    }

    [Button ("Attack")]
    public override void Attack(BaseDamagable damagable)
    {
      this.remainingDelay = this.Stat.AttackDelay;
      if (this.projectileStat.FireSound != null) {
        AudioManager.Shared.PlaySfx(
          this.projectileStat.FireSound, this.attacker.position);
      }
      var projectile = ProjectilePool.Shared.GetProjectile(this.projectileStat);
      this.InitProjectile(projectile);
      if (this.projectileStat.ExplosionSound != null) {
        projectile.OnHit = this.PlayExplosionSound;
      }
      else {
        projectile.OnHit = null;
      }
      projectile.Target = damagable;
    }

    void PlayExplosionSound(BaseProjectile projectile, Collider collider)
    {
      AudioManager.Shared.PlaySfx(
        this.projectileStat.ExplosionSound, projectile.transform.position
        );
    }

    void InitProjectile(BaseProjectile projectile)
    {
      projectile.transform.position = this.firePoint.position;
      projectile.Data = this.projectileStat;
      projectile.FiredUnit = this.attacker.gameObject;
      projectile.Damage = this.Stat.AttackDamage;
    }
  }
}
