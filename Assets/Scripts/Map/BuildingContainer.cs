using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class BuildingContainer : _MonoBehaviour
{

  [ShowInInspector]
  List<GameObject> buildings;

  public void AddBuilding(GameObject building)
  {
    this.buildings.Add(building);
    building.transform.SetParent(this.transform);
  }

  [Button("Destory all buildings")]
  void DestroyAllBuldings()
  {
    if (Application.isEditor) {
      foreach (var building in this.buildings) {
        DestroyImmediate(building); 
      }
    }
    else {
      foreach (var building in this.buildings) {
        Destroy(building); 
      }
    }
    this.buildings.Clear();
  }

  void Awake()
  {
    this.Init();
  }

  [Button("Init")]
  void Init()
  {
    this.buildings = new ();
  }

  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }
}
