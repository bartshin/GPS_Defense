using System;
using UnityEngine;
using Sirenix.OdinInspector;

public class BaseProjectile : _MonoBehaviour, IProjectile
{

  public ProjectileData Data 
  { 
    get => this.data; 
    set {
      this.data = value;
      this.OnDataChanged();
    }
  }
  [SerializeField]
  ProjectileData data;
  public Action<BaseProjectile, Collider> OnHit;
  [SerializeField]
  virtual protected Vector3 direction { get; set; }
  [SerializeField]
  virtual public int Damage { get; set; }
  [SerializeField]
  virtual public float Speed { get; set; }
  [ShowInInspector]
  virtual public BaseDamagable Target 
  { 
    get => this.target;
    set {
      this.target = value;
      this.SetDirection();
    } 
  }

  IDamagable IProjectile.Target => (IDamagable)this.target;
  protected BaseDamagable target;
  protected float remainingLifeTime; 
  public GameObject FiredUnit;
  Collider projectileCollider;

  public int CalcDamage() {
    return (this.Damage);
  }

  protected virtual void Update()
  {
    this.remainingLifeTime -= Time.deltaTime;
    this.transform.position += this.direction * this.Speed * Time.deltaTime;
    if (this.remainingLifeTime < 0) {
      this.DestroySelf();
    }
  }

  protected virtual void OnEnable()
  {
  }

  void OnDataChanged()
  {
    this.projectileCollider = this.GetComponent<Collider>();
    this.remainingLifeTime = this.Data.LifeTime;
    this.projectileCollider.includeLayers = (1 << this.Data.TargetLayer);
    this.Speed = this.Data.Speed;
  }

  private void SetDirection()
  {
    var targetPosition = this.target.transform.position;
    this.direction = (targetPosition - this.transform.position).normalized;
    this.transform.LookAt(targetPosition);
  }

  protected virtual void OnTriggerEnter(Collider collider)
  {
    if (this.OnHit != null) {
      this.OnHit.Invoke(this, collider);
    }
    var damagable = this.GetTargetFrom(collider);
    if (damagable != null) {
      if (this.FiredUnit != null &&
        this.FiredUnit.gameObject.activeSelf) {
        damagable.TakeDamage(
          attackDamage: this.Damage,
          attacker: this.FiredUnit.transform,
          attackedPosition:this.transform.position);
      }
      else {
        damagable.TakeDamage(this.Damage);
      }
    }
    this.DestroySelf();
  }


  protected IDamagable GetTargetFrom(Collider collider)
  {
    return (UnitManager.Shared.GetDamagableFrom(collider.gameObject)); 
  }

  protected virtual void DestroySelf() {
    this.gameObject.SetActive(false);
  }
}
