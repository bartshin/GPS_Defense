using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Mathematics;
using UnityEngine.Splines;
using Sirenix.OdinInspector;

[ExecuteInEditMode()]
public class SplineTest : SerializedMonoBehaviour
{
  public SplineContainer splineContainer;
  public int splieIndex;
  [Range(0, 1f)]
  public float time;
  public float3 p;
  public float3 p1;
  public float3 p2;
  public float3 forward;
  public float3 upVector;
  [Range(0.5f, 1f)]
  public float width;

  void Awake()
  {
    this.splineContainer = this.GetComponent<SplineContainer>();
  }

  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    this.splineContainer.Evaluate(
      this.splieIndex, this.time, out this.p, out this.forward, out this.upVector);

    float3 right = Vector3.Cross(
      this.forward, this.upVector
      ).normalized;
    this.p1 = this.p + right * this.width;
    this.p2 = this.p - right * this.width;
  }

  void OnDrawGizmos()
  {
    Handles.matrix = transform.localToWorldMatrix;
    Handles.SphereHandleCap(
      0, this.p1, Quaternion.identity, 1f, EventType.Repaint
      );
    Handles.SphereHandleCap(
      0, this.p2, Quaternion.identity, 1f, EventType.Repaint
      );
  }

  [Button("Create Square")]
  GameObject CreateObject()
  {
    var gameObject = new GameObject(
      "My Object", typeof(MeshRenderer));
    var meshFilter = gameObject.AddComponent<MeshFilter>();
    var mesh = new Mesh();
    mesh.name = "My Mesh";
    meshFilter.mesh = mesh;
    this.AddSquareDataTo(mesh);
    return (gameObject);
  }

  void AddSquareDataTo(Mesh mesh)
  {
    //   1---2
    //   |   |
    //   0---3 
    var vertices = new Vector3[4] {
      new Vector3(0, 0, 0),
      new Vector3(0, 1, 0),
      new Vector3(1, 1, 0),
      new Vector3(1, 0, 0)
    };
    
    // triangle[0]
    //  1---2
    //  | /
    //  0
    // triangle[1]
    //      1
    //    / |
    //  0---2
    var triangles = new int[6] {
      0, 1, 2, // triangle[0]에 해당하는 vertices의 index
      0, 2, 3 // triangle[1]에 해당하는 vertices의 index
    };
    // uv는 vertices와 순서를 맞춰야 한다
    var uv = new Vector2[4] {
      new Vector2(0, 0),
      new Vector2(0, 1),
      new Vector2(1, 1),
      new Vector2(1, 0)
    };
    mesh.vertices = vertices;
    mesh.triangles = triangles;
    mesh.uv = uv;
  }

}
