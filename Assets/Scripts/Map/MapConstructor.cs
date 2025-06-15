using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
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
  [SerializeField]
  Vector3 center;


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
  }

  [Button("Construct map")]
  void ConstructMap()
  {
    foreach (var way in this.mapReader.Ways) {
      if (way.Highway != OsmWay.HighwayType.None &&
        way.Nodes.Count > 1) {
        this.ConstructRoad(way);
      }
      else if (way.Building != null && way.Nodes.Count > 1) {
        this.ConstructBuilding(way);
      }
    }
  }

  [Button("Construct road")]
  void ConstructRoad(OsmWay roadBoundary)
  {
    var road = this.roadConstructor.Construct(roadBoundary, this.center); 
    this.roadContainer.AddRoad(road);
  }

  void DrawVertices(List<Vector3> vertices)
  {
    float radius = 0.2f;
    for (int i = 0; i < vertices.Count; i++) {
      Handles.SphereHandleCap(
        0, vertices[i], Quaternion.identity, radius, EventType.Repaint
        );
    }
  }

  [Button("Construct building from way")]
  void ConstructBuilding(OsmWay buildingBoundary)
  {
    var building = this.buildingConstructor.Construct(buildingBoundary, this.center);
    this.buildingContainer.AddBuilding(building);
  }
}
