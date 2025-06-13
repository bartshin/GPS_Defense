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
  int[] tempBuildingTriangles = new int[12];

  public GameObject Construct(OsmWay way, Vector3 mapCenter)
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
      buildingCenter.x - mapCenter.x,
      mapCenter.y,
      buildingCenter.y - mapCenter.z
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
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }
}
