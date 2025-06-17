using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBuilding : MonoBehaviour
{
  [SerializeField]
  int START_HP = 500;
  public BaseDamagable Damagable { get; private set; }

  void Awake()
  {
    this.Damagable = this.GetComponent<BaseDamagable>();
    this.Damagable.Hp.Value = (START_HP, START_HP);
  }

  void OnEnable()
  {
    this.Damagable.OnDamaged += this.OnDamaged;
  }

  void OnDisable()
  {
    this.Damagable.OnDamaged -= this.OnDamaged;
  }

  void OnDamaged()
  {
    if (this.Damagable.Hp.Value.current <= 0) {
      GameManager.Shared.GameOver();
    }
  }
}
