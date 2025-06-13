using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEditor;
using UnityEngine.Splines;
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
  Material roadMaterial;
  [SerializeField] [Range(1f, 10f)]
  float minHeight = 2f;
  [SerializeField] 
  float roadHeight = 0.2f;
  [SerializeField] [Range(0.1f, 3f)]
  float roadWidth = 1f;
  [SerializeField] [Range(1, 10)]
  int resolutionMultiplier = 1;
  [ShowInInspector]
  List<OsmWay> buildingBoundaries = new ();
  [ShowInInspector]
  List<OsmWay> roadBoundaries = new ();
  [ShowInInspector]
  List<GameObject> buildings = new ();
  List<GameObject> roads = new ();
  Vector3[] tempVertices = new Vector3[4];
  int[] tempRoadTriangles = new int[6];
  int[] tempBuildingTriangles = new int[12];
  Vector2[] tempUvs = new Vector2[4];
  float avoidZFighting = 0.001f;

  float3 tempRoadPosition;
  float3 tempRoadTangent;
  float3 tempUpVector;

  [Button("Set data")]
  void SetData()
  {
    this.buildingBoundaries.Clear();
    this.roadBoundaries.Clear();
    foreach (var way in this.mapReader.Ways) {
      if (way.Building != null && way.Nodes.Count > 1) {
        this.buildingBoundaries.Add(way);
      } 
      else if (way.Highway != OsmWay.HighwayType.None && way.Nodes.Count > 1) {
        this.roadBoundaries.Add(way);
      }
    }
  }

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
    var initialHeight = this.roadHeight;
    foreach (var roadBoundary in this.roadBoundaries) {
      var road = this.ConstructRoadsFrom(roadBoundary); 
      this.roads.Add(road);
      this.roadHeight += this.avoidZFighting;
    }
    this.roadHeight = initialHeight;
  }

  [Button("Construct road from way")]
  GameObject ConstructRoadsFrom(OsmWay way)
  {
    var gameObject = new GameObject(way.Name);
    var splineContainer = gameObject.AddComponent<SplineContainer>();
    var meshFilter = gameObject.AddComponent<MeshFilter>();
    var meshRenderer = gameObject.AddComponent<MeshRenderer>();
    var mesh = new Mesh();
    meshRenderer.material = this.roadMaterial;
    var spline = splineContainer.Splines.Count == 1 ?
      splineContainer.Splines[0]:
      splineContainer.AddSpline();
    this.InitSpline(spline, way);
    var (side1, side2) = this.CollectRoadVericies(
      way: way,
      splineContainer: splineContainer,
      splineIndex: 0);
    this.FillRoadMeshData(
      mesh: mesh,
      way: way,
      side1: side1,
      side2: side2
      );
    meshFilter.mesh = mesh;
    return (gameObject);
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

  void InitSpline(Spline spline, OsmWay way)
  {
    var knots = new List<BezierKnot>(way.Nodes.Count);
    for (int i = 0; i < way.Nodes.Count; ++i) {
      var pos = new Vector3(
        way.Nodes[i].X - this.center.x,
        this.roadHeight - this.center.y,
        way.Nodes[i].Y - this.center.z
        );
      knots.Add(new BezierKnot(pos));
    }
    spline.Knots = knots;
  }

  void FillRoadMeshData(Mesh mesh, OsmWay way, List<Vector3> side1, List<Vector3>side2)
  {
    var vertices = new List<Vector3>();
    var triangles = new List<int>();
    var uvs = new List<Vector2>();
    float uvOffset = 0f;
    float distance = 0f;
    int offset;
    int count = side1.Count - 1;
    for (int j = 0; j < count; ++j) {
      this.tempVertices[0] = side1[j]; 
      this.tempVertices[1] = side2[j]; 
      this.tempVertices[2] = side1[j + 1];
      this.tempVertices[3] = side2[j + 1];
      offset = 4 * (j);
      this.tempRoadTriangles[0] = offset + 0;
      this.tempRoadTriangles[1] = offset + 2;
      this.tempRoadTriangles[2] = offset + 3;
      this.tempRoadTriangles[3] = offset + 3;
      this.tempRoadTriangles[4] = offset + 1;
      this.tempRoadTriangles[5] = offset + 0;
      vertices.AddRange(this.tempVertices);
      triangles.AddRange(this.tempRoadTriangles);
      distance = Vector3.Distance(
        this.tempVertices[0], this.tempVertices[2] 
        ) / 4f;
      this.tempUvs[0] = new Vector2(0, uvOffset);
      this.tempUvs[1] = new Vector2(1, uvOffset);
      this.tempUvs[2] = new Vector2(0, uvOffset + distance);
      this.tempUvs[3] = new Vector2(1, uvOffset + distance);
      uvOffset += distance;
      uvs.AddRange(this.tempUvs);
    }
    mesh.vertices = vertices.ToArray();
    mesh.uv = uvs.ToArray();
    mesh.triangles = triangles.ToArray();
  }

  (List<Vector3>, List<Vector3>) CollectRoadVericies(OsmWay way, SplineContainer splineContainer, int splineIndex)
  {
    var verticesSide1 = new List<Vector3>();
    var verticesSide2 = new List<Vector3>();
    int count = splineContainer.Splines[splineIndex].Count * this.resolutionMultiplier;
    float step = 1f / (float)count;
    Vector3 right = Vector3.zero;
    Vector3 pos;
    Vector3 foward;
    (pos, foward) = this.SampleSpline(
      splineContainer: splineContainer,
      splineIndex: splineIndex,
      t: 0
      );
    foward = new Vector3(
      way.Nodes[1].X - way.Nodes[0].X,
      0,
      way.Nodes[1].Y - way.Nodes[0].Y
      ).normalized;
    right = Vector3.Cross(foward, this.tempUpVector).normalized;
    verticesSide1.Add(pos + right * this.roadWidth);
    verticesSide2.Add(pos - right * this.roadWidth);
    for (int i = 1; i < count - 1; ++i) {
      float t = step * (float)i;
      (pos, foward) = this.SampleSpline(
        splineContainer: splineContainer,
        splineIndex: splineIndex,
        t: t
        );
      right = Vector3.Cross(foward, this.tempUpVector).normalized;
      verticesSide1.Add(pos + right * this.roadWidth);
      verticesSide2.Add(pos - right * this.roadWidth);
    }
    (pos, _) = this.SampleSpline(
      splineContainer: splineContainer,
      splineIndex: splineIndex,
      t: 1
      );

    right = Vector3.Cross(foward, this.tempUpVector).normalized;
    verticesSide1.Add(pos + right * this.roadWidth);
    verticesSide2.Add(pos - right * this.roadWidth);
    return (verticesSide1, verticesSide2);
  }

  (Vector3 pos, Vector3 foward) SampleSpline(SplineContainer splineContainer, int splineIndex, float t)
  {
    splineContainer.Evaluate(
      splineIndex: splineIndex,
      t: t,
      position: out this.tempRoadPosition,
      tangent: out this.tempRoadTangent,
      out this.tempUpVector
      );
    return (
      new Vector3(this.tempRoadPosition.x, this.tempRoadPosition.y, this.tempRoadPosition.z),
      new Vector3(this.tempRoadTangent.x, this.tempRoadTangent.y, this.tempRoadTangent.z)
      );
  }

  [Button("Construct building from way")]
  GameObject ConstructBuildingFrom(OsmWay way)
  {
    var gameObject = new GameObject(way.Name);
    var meshFilter = gameObject.AddComponent<MeshFilter>();
    var meshRenderer = gameObject.AddComponent<MeshRenderer>();
    var mesh = new Mesh();
    var buildingCenter = way.CalcCenter();
    this.FillBuildingMeshData(mesh, way, buildingCenter);
    meshFilter.mesh = mesh;
    meshRenderer.material = this.buildingMaterial;
    gameObject.transform.position = new Vector3(
      buildingCenter.x - this.center.x,
      this.center.y,
      buildingCenter.y - this.center.z
      );
    return (gameObject);
  }

  void FillBuildingMeshData(Mesh mesh, OsmWay way, Vector2 buildingCenter)
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

      this.tempBuildingTriangles[0] = triangleOffset + 0;
      this.tempBuildingTriangles[1] = triangleOffset + 2;
      this.tempBuildingTriangles[2] = triangleOffset + 1;

      this.tempBuildingTriangles[3] = triangleOffset + 2;
      this.tempBuildingTriangles[4] = triangleOffset + 3;
      this.tempBuildingTriangles[5] = triangleOffset + 1;

      this.tempBuildingTriangles[6] = triangleOffset + 1;
      this.tempBuildingTriangles[7] = triangleOffset + 2;
      this.tempBuildingTriangles[8] = triangleOffset + 0;

      this.tempBuildingTriangles[9] = triangleOffset + 1;
      this.tempBuildingTriangles[10] = triangleOffset + 3;
      this.tempBuildingTriangles[11] = triangleOffset + 2;

      vertices.AddRange(this.tempVertices);
      triangles.AddRange(this.tempBuildingTriangles);
    }
    mesh.vertices = vertices.ToArray();
    mesh.triangles = triangles.ToArray();
    mesh.normals = normals.ToArray();
  }
}
