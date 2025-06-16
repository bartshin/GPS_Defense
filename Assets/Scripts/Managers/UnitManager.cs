using System.Collections.Generic;
using UnityEngine;
using Architecture;

public class UnitManager: SingletonBehaviour<UnitManager>
{

  public BaseDamagable GetDamagableFrom(GameObject gameObject)
  {
    //TODO: Create dictionary
    return (gameObject.GetComponent<BaseDamagable>());
  }

  public void ClearDamagables()
  {

  }

  protected override void Awake()
  {
    base.Awake();
  }
}
