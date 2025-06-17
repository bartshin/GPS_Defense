using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using Sirenix.OdinInspector;

public class RoadContainer : _MonoBehaviour
{
  [SerializeField] [Required(InfoMessageType.Error)]
  GameObject floor;
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

  [Button("Set floor size")]
  public void SetFloorSize(Vector2 size)
  {
    this.floor.transform.position = Vector3.zero;
    this.floor.transform.localScale = new Vector3(
      size.x, 1, size.y
      );
  }

  public void AddRoad(GameObject road)
  {
    road.transform.SetParent(this.transform);
    road.layer = this.pathLayer;
    this.roads.Add(road);
  }

  [Button("Update NavMeshSurface")]
  public void UpdateNavMesh()
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
