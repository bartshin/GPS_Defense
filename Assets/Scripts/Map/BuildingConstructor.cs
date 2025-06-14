using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using OSM;

public class BuildingConstructor : _MonoBehaviour
{
  struct MeshData
  {
    public List<Vector3> Vertices;
    public List<int> Triangles;
    public List<Vector3> Normals;
    public List<Vector2> Uvs;
      
    public MeshData(int boundaryCount) 
    {
      this.Vertices = new List<Vector3>((boundaryCount - 1) * 4 + 1);
      this.Triangles = new List<int>((boundaryCount - 1) * (12 + 3));
      this.Normals = new List<Vector3>((boundaryCount - 1) * 4);
      this.Uvs = new List<Vector2>();
    }
  }
  [SerializeField]
  Material buildingMaterial;
  [SerializeField] [Range(1f, 10f)]
  float minHeight = 2f;
  Vector3[] tempVertices = new Vector3[4];
  int[] tempTriangles = new int[6];
  Vector2[] tempUvs = new Vector2[4];

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
    var meshData = new MeshData(way.Nodes.Count);
    this.CollectGeometries(
      data: meshData,
      way: way,
      buildingCenter: buildingCenter,
      height: height
      );
    this.AddTopGeomeries(
      data: meshData,
      buildingCenter: buildingCenter,
      buildingBounds: way.Bounds,
      height: height,
      wayCount: way.Nodes.Count);
    mesh.vertices = meshData.Vertices.ToArray();
    mesh.triangles = meshData.Triangles.ToArray();
    mesh.normals = meshData.Normals.ToArray();
    mesh.uv = meshData.Uvs.ToArray();
  }

  void CollectGeometries(MeshData data, OsmWay way, Vector2 buildingCenter, float height)
  {
    this.tempUvs[0] = new Vector2(0, 0);
    this.tempUvs[1] = new Vector2(1, 0);
    this.tempUvs[2] = new Vector2(0, 1);
    this.tempUvs[3] = new Vector2(1, 1);
    var heightVector = new Vector3(0, height, 0);
    int triangleOffset = 0;
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

      data.Normals.Add(-Vector3.forward);
      data.Normals.Add(-Vector3.forward);
      data.Normals.Add(-Vector3.forward);
      data.Normals.Add(-Vector3.forward);

      triangleOffset = 4 * (i - 1);

      this.tempTriangles[0] = triangleOffset + 0;
      this.tempTriangles[1] = triangleOffset + 2;
      this.tempTriangles[2] = triangleOffset + 1;

      this.tempTriangles[3] = triangleOffset + 2;
      this.tempTriangles[4] = triangleOffset + 3;
      this.tempTriangles[5] = triangleOffset + 1;

      data.Vertices.AddRange(this.tempVertices);
      data.Triangles.AddRange(this.tempTriangles);
      data.Uvs.AddRange(this.tempUvs);
    }
  }

  void AddTopGeomeries(
    MeshData data,
    Vector2 buildingCenter,
    OsmBounds buildingBounds,
    float height,
    int wayCount)
  {
    Vector3 topCenter = new Vector3(0, height, 0);
    Vector2 range = buildingBounds.CalcRange();
    Vector2 centerUv = new Vector2(0.5f, 0.5f);
    for (int i = 1; i < wayCount; i++) {
      int baseIndex = 4 * (i - 1);
      this.tempVertices[0] = data.Vertices[baseIndex + 3];
      this.tempVertices[1] = data.Vertices[baseIndex + 2];

      data.Vertices.Add(this.tempVertices[0]); 
      data.Vertices.Add(this.tempVertices[1]); 
      data.Vertices.Add(topCenter);
      data.Triangles.Add(data.Vertices.Count - 3); 
      data.Triangles.Add(data.Vertices.Count - 2); 
      data.Triangles.Add(data.Vertices.Count - 1); 
      data.Normals.Add(Vector3.up);
      data.Normals.Add(Vector3.up);
      data.Normals.Add(Vector3.up);
      
      data.Uvs.Add(
        new Vector2(
          this.tempVertices[0].x / range.x + 0.5f,
          this.tempVertices[0].z / range.y + 0.5f
          )
        );
      data.Uvs.Add(
        new Vector2(
          this.tempVertices[1].x / range.x + 0.5f,
          this.tempVertices[1].z / range.y + 0.5f
          )
        );
      data.Uvs.Add(centerUv);
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
