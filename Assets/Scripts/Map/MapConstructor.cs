using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OSM;
using Sirenix.OdinInspector;

[RequireComponent(typeof(MapReader), typeof(MeshFilter), typeof(MeshRenderer))]
public class MapConstructor : MonoBehaviour
{
  [SerializeField] [Required(InfoMessageType.Error)]
  MapReader mapReader;
  MeshFilter meshFilter;
  MeshRenderer meshRenderer;
  [SerializeField]
  Vector3 center;
  [SerializeField]
  Material buildingMaterial;
  [SerializeField]
  float minHeight = 2f;
  [ShowInInspector]
  List<OsmWay> buildingBoundaries = new ();
  List<GameObject> buildings = new ();
  Vector3[] tempVertices = new Vector3[4];
  int[] tempTriangles = new int[12];

  [Button("Set building data")]
  void SetBuildingData()
  {
    this.buildingBoundaries.Clear();
    foreach (var way in this.mapReader.Ways) {
      if (way.Building != null && way.Nodes.Count > 1) {
        this.buildingBoundaries.Add(way);
      } 
    }
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

  [Button("Construct buildings")]
  void ConstructBuildings()
  {
    foreach (var way in this.mapReader.Ways) {
      if (way.Building != null && way.Nodes.Count > 1) {
        this.buildings.Add(this.ConstructBuildingFrom(way)); 
      } 
    }
  }

  [Button("Construct roads")]
  void ConstructRoads()
  {

  }

  [Button("Construct building from way")]
  GameObject ConstructBuildingFrom(OsmWay way)
  {
    var gameObject = new GameObject(way.Name);
    var meshFilter = gameObject.AddComponent<MeshFilter>();
    var meshRenderer = gameObject.AddComponent<MeshRenderer>();
    var mesh = new Mesh();
    var buildingCenter = way.CalcCenter();
    this.FillMeshData(mesh, way, buildingCenter);
    meshFilter.mesh = mesh;
    meshRenderer.material = this.buildingMaterial;
    gameObject.transform.position = new Vector3(
      buildingCenter.x - this.center.x,
      this.center.y,
      buildingCenter.y - this.center.z
      );
    return (gameObject);
  }

  void FillMeshData(Mesh mesh, OsmWay way, Vector2 buildingCenter)
  {
    var height = new Vector3(0, Math.Max(way.Height, this.minHeight), 0);
    int triangleOffset = 0;
    var vertices = new List<Vector3>((way.Nodes.Count - 1) * 4);
    var triangles = new List<int>((way.Nodes.Count - 1) * 12);
    var normals = new List<Vector3>((way.Nodes.Count - 1) * 4);
    var count = way.Nodes.Count;
    for (int i = 1; i < count; ++i) {
      this.tempVertices[0] = new Vector3(
        way.Nodes[i - 1].X - buildingCenter.x,
        0,
        way.Nodes[i - 1].Y - buildingCenter.y
        );
      this.tempVertices[1] = new Vector3(
        way.Nodes[i].X - buildingCenter.x,
        0,
        way.Nodes[i].Y - buildingCenter.y
        );
      this.tempVertices[2] = this.tempVertices[0] + height;
      this.tempVertices[3] = this.tempVertices[1] + height; 

      normals.Add(-Vector3.forward);
      normals.Add(-Vector3.forward);
      normals.Add(-Vector3.forward);
      normals.Add(-Vector3.forward);
      
      // 바깥쪽이 어느쪽인지 정보가 없음
      triangleOffset = 4 * (i - 1);
      
      this.tempTriangles[0] = triangleOffset + 0;
      this.tempTriangles[1] = triangleOffset + 2;
      this.tempTriangles[2] = triangleOffset + 1;

      this.tempTriangles[3] = triangleOffset + 2;
      this.tempTriangles[4] = triangleOffset + 3;
      this.tempTriangles[5] = triangleOffset + 1;

      this.tempTriangles[6] = triangleOffset + 1;
      this.tempTriangles[7] = triangleOffset + 2;
      this.tempTriangles[8] = triangleOffset + 0;

      this.tempTriangles[9] = triangleOffset + 1;
      this.tempTriangles[10] = triangleOffset + 3;
      this.tempTriangles[11] = triangleOffset + 2;

      vertices.AddRange(this.tempVertices);
      triangles.AddRange(this.tempTriangles);
    }
    mesh.vertices = vertices.ToArray();
    mesh.triangles = triangles.ToArray();
    mesh.normals = normals.ToArray();
  }
}
