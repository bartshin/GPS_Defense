using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using Sirenix.OdinInspector;

public class RoadContainer : _MonoBehaviour
{
  [ShowInInspector]
  List<GameObject> roads;
  [SerializeField] [Required(InfoMessageType.Error)]
  NavMeshSurface navMeshSurface;
  int pathLayer;

  [Button("Destory all roads")]
  void DestroyAllRoads()
  {
    if (Application.isEditor) {
      foreach (var road in this.roads) {
        DestroyImmediate(road); 
      }
    }
    else {
      foreach (var road in this.roads) {
        Destroy(road); 
      }
    }
    this.roads.Clear();
  }

  public void AddRoad(GameObject road)
  {
    road.transform.SetParent(this.transform);
    road.layer = this.pathLayer;
    this.roads.Add(road);
  }

  [Button("Create NavMeshSurface")]
  public void CreateNavMesh()
  {
    this.navMeshSurface.BuildNavMesh();
  }

  void Awake()
  {
    this.Init();
  }

  [Button("Init")]
  void Init()
  {
    this.roads = new();
    this.pathLayer = LayerMask.NameToLayer("Path");
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
