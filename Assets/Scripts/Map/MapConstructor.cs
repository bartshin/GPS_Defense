using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using OSM;
using Sirenix.OdinInspector;

[RequireComponent(typeof(MapReader), typeof(MeshFilter), typeof(MeshRenderer))]
public class MapConstructor : MonoBehaviour
{
  [SerializeField] [Required(InfoMessageType.Error)]
  MapReader mapReader;
  [SerializeField] [Required(InfoMessageType.Error)]
  RoadConstructor roadConstructor;
  [SerializeField] [Required(InfoMessageType.Error)]
  BuildingConstructor buildingConstructor;
  MeshFilter meshFilter;
  MeshRenderer meshRenderer;
  [SerializeField]
  Vector3 center;
  [ShowInInspector]
  List<GameObject> buildings = new ();
  [ShowInInspector]
  List<GameObject> roads = new ();
  GameObject container;

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
    if (this.meshFilter == null) {
      this.meshFilter = this.GetComponent<MeshFilter>();
    }
    if (this.meshRenderer == null) {
      this.meshRenderer = this.GetComponent<MeshRenderer>();
    }
    this.container = new GameObject("Map object container");
  }

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
    foreach (var way in this.mapReader.Ways) {
      if (way.Highway != OsmWay.HighwayType.None &&
        way.Nodes.Count > 1) {
        //this.ConstructRoad(way);
      }
      else if (way.Building != null && way.Nodes.Count > 1) {
        this.ConstructBuilding(way);
      }
    }
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

  [Button("Construct road")]
  void ConstructRoad(OsmWay roadBoundary)
  {
    var road = this.roadConstructor.Construct(roadBoundary, this.center); 
    road.transform.parent = this.container.transform;
    this.roads.Add(road);
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
    this.buildings.Add(building);
    building.transform.parent = this.container.transform;
  }
}
