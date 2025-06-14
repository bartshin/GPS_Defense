using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using OSM;

public class BuildingConstructor : _MonoBehaviour
{
  [SerializeField]
  Material buildingMaterial;
  [SerializeField] [Range(1f, 10f)]
  float minHeight = 2f;
  Vector3[] tempVertices = new Vector3[4];
  //int[] tempBuildingTriangles = new int[12];
  int[] tempBuildingTriangles = new int[6];

  public GameObject Construct(OsmWay way, Vector3 mapCenter)
  {
    var gameObject = new GameObject(way.Name);
    var meshFilter = gameObject.AddComponent<MeshFilter>();
    var meshRenderer = gameObject.AddComponent<MeshRenderer>();
    var mesh = new Mesh();
    var buildingCenter = way.CalcCenter();
    var isClockWise = this.IsClockWise(way.Nodes);
    if (!isClockWise) {
      way.Nodes.Reverse();
    }
    this.FillBuildingMeshData(mesh, way, buildingCenter);
    meshFilter.mesh = mesh;
    meshRenderer.material = this.buildingMaterial;
    gameObject.transform.position = new Vector3(
      buildingCenter.x - mapCenter.x,
      mapCenter.y,
      buildingCenter.y - mapCenter.z
      );
    return (gameObject);
  }

  void FillBuildingMeshData(Mesh mesh, OsmWay way, Vector2 buildingCenter)
  {
    var height = Math.Max(way.Height, this.minHeight);
    var (vertices, triangles, normals) = this.CollectGeometries(
      way: way,
      buildingCenter: buildingCenter,
      height: height);
    this.AddTopGeomeries(
      vertices: vertices,
      triangles: triangles,
      normals: normals,
      buildingCenter: buildingCenter,
      height: height,
      wayCount: way.Nodes.Count);
    mesh.vertices = vertices.ToArray();
    mesh.triangles = triangles.ToArray();
    mesh.normals = normals.ToArray();
  }

  (List<Vector3> vertices, List<int> triangles, List<Vector3> normals)
    CollectGeometries(OsmWay way, Vector2 buildingCenter, float height)
  {
    var heightVector = new Vector3(0, height, 0);
    int triangleOffset = 0;
    var vertices = new List<Vector3>((way.Nodes.Count - 1) * 4 + 1);
    var triangles = new List<int>((way.Nodes.Count - 1) * (12 + 3));
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
      this.tempVertices[2] = this.tempVertices[0] + heightVector;
      this.tempVertices[3] = this.tempVertices[1] + heightVector; 

      normals.Add(-Vector3.forward);
      normals.Add(-Vector3.forward);
      normals.Add(-Vector3.forward);
      normals.Add(-Vector3.forward);

      triangleOffset = 4 * (i - 1);

      this.tempBuildingTriangles[0] = triangleOffset + 0;
      this.tempBuildingTriangles[1] = triangleOffset + 2;
      this.tempBuildingTriangles[2] = triangleOffset + 1;

      this.tempBuildingTriangles[3] = triangleOffset + 2;
      this.tempBuildingTriangles[4] = triangleOffset + 3;
      this.tempBuildingTriangles[5] = triangleOffset + 1;

      vertices.AddRange(this.tempVertices);
      triangles.AddRange(this.tempBuildingTriangles);
    }
    return (vertices, triangles, normals);
  }

  void AddTopGeomeries(
    List<Vector3> vertices, 
    List<int> triangles,
    List<Vector3> normals,
    Vector2 buildingCenter,
    float height,
    int wayCount)
  {
    Vector3 topCenter = new Vector3(0, height, 0);
    Vector3 v1 = Vector3.zero;
    Vector3 v2 = Vector3.zero;
    for (int i = 1; i < wayCount; i++) {
      int baseIndex = 4 * (i - 1);

      vertices.Add(vertices[baseIndex + 3]); 
      vertices.Add(vertices[baseIndex + 2]); 
      vertices.Add(topCenter);
      triangles.Add(vertices.Count - 3); 
      triangles.Add(vertices.Count - 2); 
      triangles.Add(vertices.Count - 1); 
      normals.Add(Vector3.up);
      normals.Add(Vector3.up);
      normals.Add(Vector3.up);
    }
  }

  //https://stackoverflow.com/questions/1165647/how-to-determine-if-a-list-of-polygon-points-are-in-clockwise-order
  bool IsClockWise(List<OsmWay.WayNode> nodes)
  {
    float sumOfEdge = 0f;
    for (int i = 0; i < nodes.Count - 1; ++i) {
      sumOfEdge += (nodes[i + 1].X - nodes[i].X) * (nodes[i + 1].Y + nodes[i].Y); 
    }
    sumOfEdge += (nodes[0].X - nodes[nodes.Count - 1].X) * (nodes[0].Y + nodes[nodes.Count - 1].Y);
    return (sumOfEdge < 0f);
  }
}
