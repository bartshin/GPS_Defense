using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using Sirenix.OdinInspector;
using Road;

public class RoadDataLoader : MonoBehaviour
{
  [SerializeField] [FilePath(AbsolutePath = true, ParentFolder = "Assets/Data", Extensions = "json")]
  string mapDataPath;
  [ShowInInspector]
  List<RoadData> roadData;
  [SerializeField]
  float roadHeight;
  [SerializeField]
  float zoomLevel = 100f;
  [ShowInInspector]
  Coordinates center 
  {
    get => this._center;
    set {
      this._center = value;
      GPSConverter.Shared.Init(value, this.zoomLevel);
    }
  }
  Coordinates _center = new Coordinates {Lat = 37.5203451f, Lng = 126.9939702f };
   
  [Button("Load Json")]
  void LoadJson()
  {
    string jsonString = File.ReadAllText(this.mapDataPath);
    var jsonData = JsonUtility.FromJson<Road.JsonData>(jsonString);
    this.roadData = MapDataParser.Shared.ParseRoadData(jsonData);
  }

  [Button("Create Splines")]
  GameObject CreateSplines()
  {
    GPSConverter.Shared.Init(this.center, this.zoomLevel);
    var gameObject = new GameObject("Road");
    var splineContainer = gameObject.AddComponent<SplineContainer>();

    for (int i = 0; i < this.roadData.Count; i++) {
      var positions = GPSConverter.Shared.Convert(this.roadData[i].Path); 
      var spline = splineContainer.AddSpline();
      var knots = new BezierKnot[positions.Length];
      for (int j = 0; j < positions.Length; ++j) {
        knots[j]  = new BezierKnot(new Vector3(
            positions[j].x,
            this.roadHeight,
            positions[j].y
            ));
      }
      spline.Knots = knots;
    }
    var roadConstructor = gameObject.AddComponent<RoadConstructor>();
    return (gameObject);
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
