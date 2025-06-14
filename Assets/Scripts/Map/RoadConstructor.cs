using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using OSM;
using Sirenix.OdinInspector;

public class RoadConstructor : _MonoBehaviour
{
  struct MeshData
  {
    public List<Vector3> Vertice;
    public List<Vector2> Uvs;
    public List<int> Triangles;
  }
  [SerializeField]
  Material material;
  [SerializeField] 
  float height = 0.2f;
  [SerializeField] [Range(0.1f, 3f)]
  float width = 1f;
  [SerializeField] [Range(1, 10)]
  int resolutionMultiplier = 1;
  SplineContainer splineContainer;
  Vector3[] tempVertices = new Vector3[4];
  Vector2[] tempUvs = new Vector2[4];
  int[] tempTriangles = new int[6];
  float avoidZFighting = 0.001f;
  float3 tempPosition;
  float3 tempTangent;
  float3 tempUpVector;
  MeshRenderer meshRenderer;
  MeshFilter meshFilter;


  public GameObject Construct(OsmWay way, Vector3 mapCenter)
  {
    var road = new GameObject(way.Name);
    var meshFilter = road.AddComponent<MeshFilter>();
    var meshRenderer = road.AddComponent<MeshRenderer>();
    var mesh = new Mesh();
    meshRenderer.material = this.material;
    var spline = this.splineContainer.AddSpline();
    this.InitSpline(
      spline: spline,
      way: way,
      mapCenter: mapCenter
      );
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
    this.height += this.avoidZFighting;
    this.splineContainer.RemoveSplineAt(0);
    return (road);
  }

  void Awake()
  {
    this.Init();
  }

  [Button("Init")]
  void Init()
  {
    this.splineContainer = this.gameObject.AddComponent<SplineContainer>();
    for (int i = this.splineContainer.Splines.Count - 1; i >= 0; --i) {
      this.splineContainer.RemoveSplineAt(i); 
    }
  }

  void InitSpline(Spline spline, OsmWay way, Vector3 mapCenter)
  {
    var knots = new List<BezierKnot>(way.Nodes.Count);
    for (int i = 0; i < way.Nodes.Count; ++i) {
      var pos = new Vector3(
        way.Nodes[i].X - mapCenter.x,
        this.height - mapCenter.y,
        way.Nodes[i].Y - mapCenter.z
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
    verticesSide1.Add(pos + right * this.width);
    verticesSide2.Add(pos - right * this.width);
    for (int i = 1; i < count - 1; ++i) {
      float t = step * (float)i;
      (pos, foward) = this.SampleSpline(
        splineContainer: splineContainer,
        splineIndex: splineIndex,
        t: t
        );
      right = Vector3.Cross(foward, this.tempUpVector).normalized;
      verticesSide1.Add(pos + right * this.width);
      verticesSide2.Add(pos - right * this.width);
    }
    (pos, _) = this.SampleSpline(
      splineContainer: splineContainer,
      splineIndex: splineIndex,
      t: 1
      );

    right = Vector3.Cross(foward, this.tempUpVector).normalized;
    verticesSide1.Add(pos + right * this.width);
    verticesSide2.Add(pos - right * this.width);
    return (verticesSide1, verticesSide2);
  }

  (Vector3 pos, Vector3 foward) SampleSpline(SplineContainer splineContainer, int splineIndex, float t)
  {
    splineContainer.Evaluate(
      splineIndex: splineIndex,
      t: t,
      position: out this.tempPosition,
      tangent: out this.tempTangent,
      out this.tempUpVector
      );
    return (
      new Vector3(this.tempPosition.x, this.tempPosition.y, this.tempPosition.z),
      new Vector3(this.tempTangent.x, this.tempTangent.y, this.tempTangent.z)
      );
  }

}
