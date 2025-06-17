using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBuilding : MonoBehaviour
{
  BaseDamagable damagable;  

  void Awake()
  {
    this.damagable = this.GetComponent<BaseDamagable>();
  }

  void OnEnable()
  {
    this.damagable.OnDamaged += this.OnDamaged;
  }

  void OnDisable()
  {
    this.damagable.OnDamaged -= this.OnDamaged;
  }

  void OnDamaged()
  {
    if (this.damagable.Hp.Value.current <= 0) {
      GameManager.Shared.GameOver();
    }
  }
}
