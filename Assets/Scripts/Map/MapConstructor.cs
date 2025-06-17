using System.Collections;
using UnityEngine;
using OSM;
using Sirenix.OdinInspector;

public class MapConstructor : MonoBehaviour
{
  [SerializeField] [Required(InfoMessageType.Error)]
  MapReader mapReader;
  [SerializeField] [Required(InfoMessageType.Error)]
  RoadConstructor roadConstructor;
  [SerializeField] [Required(InfoMessageType.Error)]
  BuildingConstructor buildingConstructor;
  [SerializeField] [Required(InfoMessageType.Error)]
  RoadContainer roadContainer; 
  [SerializeField] [Required(InfoMessageType.Error)]
  BuildingContainer buildingContainer;
  [SerializeField] [Required(InfoMessageType.Error)]
  GameObject mainBuildingPrefab;
  int constructingCountAtOcne = 40;
  [SerializeField]
  Vector3 center;
  Vector2 center2d;
  float minDistToCenter = 100f;

  void OnEnable()
  {
    this.mapReader.OnFinishRead += this.OnFinishReadMap;
  }

  void OnDisable()
  {
    this.mapReader.OnFinishRead -= this.OnFinishReadMap;
  }

  void OnFinishReadMap()
  {
    this.UpdateCenter();
    this.ConstructMap();
  }

  [Button("Update center")]
  void UpdateCenter()
  {
    this.center = new Vector3(
      this.mapReader.Bounds.Center.x,
      0,
      this.mapReader.Bounds.Center.y
      );
    this.center2d = new Vector3(
      this.center.x,
      this.center.y
      );
  }

  [Button("Construct map")]
  void ConstructMap()
  {
    this.StartCoroutine(this.MapConstructRoutine());
  }

  IEnumerator MapConstructRoutine()
  {
    var mainBuilding = this.ConstructMainBuilding();
    GameManager.Shared.MainBuilding = mainBuilding;
    yield return (null);
    Vector2 mapSize = this.mapReader.Bounds.CalcRange(); 
    this.roadContainer.SetFloorSize(mapSize * 0.3f);
    foreach (var way in this.mapReader.Ways) {
      int count = 0;
      if (way.Highway != OsmWay.HighwayType.None &&
        way.Nodes.Count > 1) {
          this.ConstructRoad(way);
      }
      else if (way.Building != null && way.Nodes.Count > 1) {
          this.ConstructBuilding(way);
      }
      if (count++ % this.constructingCountAtOcne  == 0) {
        yield return (null);
      }
    }
    yield return (null);
    this.roadContainer.UpdateNavMesh();
    GameManager.Shared.OnFinishLoading();
  }
  

  [Button("Construct road")]
  void ConstructRoad(OsmWay roadBoundary)
  {
    var road = this.roadConstructor.Construct(roadBoundary, this.center); 
    this.roadContainer.AddRoad(road);
  }

  [Button("Construct building from way")]
  void ConstructBuilding(OsmWay buildingBoundary)
  {
    var building = this.buildingConstructor.Construct(buildingBoundary, this.center);
    this.buildingContainer.AddBuilding(building);
  }

  MainBuilding ConstructMainBuilding()
  {
    var building = Instantiate(
      this.mainBuildingPrefab, Vector3.zero, Quaternion.identity);
    building.name = "MainBuilding";
    this.buildingContainer.AddBuilding(building);
    return (building.GetComponent<MainBuilding>());
  }
}
