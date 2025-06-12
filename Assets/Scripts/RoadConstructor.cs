using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Mathematics;
using UnityEngine.Splines;    
using Sirenix.OdinInspector;

[ExecuteInEditMode()]
public class RoadConstructor : MonoBehaviour
{

  [SerializeField] [Range(10, 100)]
  int resolution;
  [SerializeField] [Range(0f, 3f)]
  float width;
  [SerializeField] [Range(0f, 3f)]
  float radius;

  float3 position;
  float3 tangent;
  float3 upVector;
  [ShowInInspector]
  List<Vector3> side1;
  [ShowInInspector]
  List<Vector3> side2;

  [SerializeField]
  MeshFilter meshFilter;

  Vector3[] tempVertices = new Vector3[4];
  int[] tempTriangles = new int[6];
  Vector2[] tempUvs = new Vector2[4];

  [Button("ConstructRoad")]
  public void ConstructRoadFrom(SplineContainer splineContainer)
  {
    if (splineContainer == null) {
      splineContainer = this.GetComponent<SplineContainer>();
    }
    this.side1 = new();
    this.side2 = new();
    var mesh = new Mesh();
    for (int i = 0; i < splineContainer.Splines.Count; i++) {
      this.CollectVertices(
        splineContainer: splineContainer,
        index: i);
    }
    this.AddMeshData(mesh, splineContainer.Splines.Count);
    this.meshFilter.mesh = mesh;
  }

  void CollectVertices(SplineContainer splineContainer, int index)
  {
    float step = 1f / (float)this.resolution;
    Vector3 pos = Vector3.zero;
    Vector3 foward = Vector3.zero;
    Vector3 right;
    for (int i = 0; i < this.resolution; ++i) {
      float t = step * (float)i; 
      this.SampleSpline(
        splineContainer, index, t, ref pos, ref foward
        );
      right = Vector3.Cross(foward, this.upVector).normalized;
      this.side1.Add(pos + right * this.width);
      this.side2.Add(pos + right * this.width * -1);
    }
    this.SampleSpline(
      splineContainer, index, 1, ref pos, ref foward
      );
    right = Vector3.Cross(foward, this.upVector).normalized;
    this.side1.Add(pos + right * this.width);
    this.side2.Add(pos + right * this.width * -1);
  }

  void SampleSpline(SplineContainer splineContainer, int splineIndex, float t, ref Vector3 pos, ref Vector3 foward)
  {
    splineContainer.Evaluate(
      splineIndex: splineIndex,
      t: t,
      position: out this.position,
      tangent: out this.tangent,
      out this.upVector
      );
    pos.Set(this.position.x, this.position.y, this.position.z);
    foward.Set(this.tangent.x, this.tangent.y, this.tangent.z);
  }

  void OnDrawGizmos()
  {
    Handles.matrix = transform.localToWorldMatrix;
    if (this.side1 != null) {
      this.DrawVertices(this.side1);
    }
    if (this.side2 != null) {
      this.DrawVertices(this.side2);
    }
  }

  void AddMeshData(Mesh mesh, int numberOfSplines)
  {
    var vertices = new List<Vector3>();
    var triangles = new List<int>();
    var uvs = new List<Vector2>();
    float uvOffset = 0f;
    float distance = 0f;
    for (int splineIndex = 0; splineIndex < numberOfSplines; splineIndex++) {
      int splineOffset = this.resolution * splineIndex + splineIndex;
      for (int j = 0; j < this.resolution; ++j) {
        int vertIndex = splineOffset + j;
        this.tempVertices[0] = this.side1[vertIndex]; 
        this.tempVertices[1] = this.side2[vertIndex]; 
        this.tempVertices[2] = this.side1[vertIndex + 1];
        this.tempVertices[3] = this.side2[vertIndex + 1];
        int offset = 4 * (this.resolution * splineIndex + j);
        this.tempTriangles[0] = offset + 0;
        this.tempTriangles[1] = offset + 2;
        this.tempTriangles[2] = offset + 3;
        this.tempTriangles[3] = offset + 3;
        this.tempTriangles[4] = offset + 1;
        this.tempTriangles[5] = offset + 0;
        vertices.AddRange(this.tempVertices);
        triangles.AddRange(this.tempTriangles);
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
    }
    mesh.vertices = vertices.ToArray();
    mesh.subMeshCount = numberOfSplines;
    mesh.uv = uvs.ToArray();
    int trianglesCount = this.resolution * 6;
    var subTriangles = new int[trianglesCount];
    for (int i = 0; i < numberOfSplines; i++) {
      triangles.CopyTo(trianglesCount * i, subTriangles, 0, trianglesCount);
      mesh.SetTriangles(subTriangles, i); 
    }
  }

  void DrawVertices(List<Vector3> vertices)
  {
    if (this.radius == 0) {
      return;
    }
    for (int i = 0; i < vertices.Count; i++) {
      Handles.SphereHandleCap(
        0, vertices[i], Quaternion.identity, this.radius, EventType.Repaint
        );
    }
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
