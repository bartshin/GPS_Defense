using System;
using UnityEngine;
using Sirenix.OdinInspector;
using Architecture;

public class BaseProjectile : _MonoBehaviour, IProjectile, IPooledObject
{

  public ProjectileStat Data 
  { 
    get => this.data; 
    set {
      this.data = value;
      this.OnDataChanged();
    }
  }
  [SerializeField]
  ProjectileStat data;
  public Action<BaseProjectile, Collider> OnHit;
  [SerializeField]
  virtual protected Vector3 direction { get; set; }
  [SerializeField]
  virtual public int Damage { get; set; }
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

  public Action<IPooledObject> OnDisabled { get; set; }

  protected BaseDamagable target;
  protected float remainingLifeTime; 
  public GameObject FiredUnit;
  Collider projectileCollider;

  public int CalcDamage() {
    return (this.Damage);
  }

  protected virtual void Awake()
  {
    this.projectileCollider = this.GetComponent<Collider>();
  }

  protected virtual void Update()
  {
    this.remainingLifeTime -= Time.deltaTime;
    this.transform.position += this.direction * this.Data.Speed * Time.deltaTime;
    if (this.remainingLifeTime < 0) {
      this.DestroySelf();
    }
  }

  protected virtual void OnEnable()
  {
    if (this.Data == null) {
      this.remainingLifeTime = 2f;
    }
    else {
      this.remainingLifeTime = this.Data.LifeTime;
    }
  }

  void OnDataChanged()
  {
    this.remainingLifeTime = this.Data.LifeTime;
    this.projectileCollider.includeLayers = (this.Data.TargetLayer.value);
    this.projectileCollider.excludeLayers = ~(this.Data.TargetLayer.value);
  }

  private void SetDirection()
  {
    var targetPosition = this.target.transform.position;
    this.direction = new Vector3(
      targetPosition.x - this.transform.position.x,
      0,
      (targetPosition.z - this.transform.position.z)
      ).normalized;
    this.transform.forward = this.direction;
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
    if (this.OnDisabled != null) {
      this.OnDisabled.Invoke(this);
    }
    else {
      this.gameObject.SetActive(false);
    }
  }
}
