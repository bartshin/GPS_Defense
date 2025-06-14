using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using UnityEngine;
using OSM;
using Sirenix.OdinInspector;

public class MapReader : _MonoBehaviour
{
  [SerializeField]
  public OsmBounds Bounds { get; private set; }
  [ShowInInspector] 
  public Dictionary<ulong, OsmNode> Nodes { get; private set; }
  [ShowInInspector]
  public List<OsmWay> Ways { get; private set; }
  public Action OnFinishRead;

  [SerializeField] [FilePath(AbsolutePath = true, ParentFolder = "Assets/Data", Extensions = "xml")]
  string dataPath;
  [SerializeField]
  (float min, float max) latitudeRange
  {
    get => this._latitudeRange;
    set {
      this._latitudeRange = value;
      if (this.longitudeRange != (0, 0)) {
        this.SetBounds();
      }
    }
  }
  [SerializeField]
  (float min, float max) longitudeRange
  {
    get => this._longitudeRange;
    set {
      this._longitudeRange = value;
      if (this.latitudeRange != (0, 0)) {
        this.SetBounds();
      }
    }
  }

  (float min, float max) _latitudeRange;
  (float min, float max) _longitudeRange;

  [SerializeField]
  bool drawWays;
  
  [Button("Clear data")]
  void ClearData()
  {
    this.Nodes?.Clear();
    this.Ways?.Clear();
  }

  [Button("Read data File")]
  void ReadDataFile()
  {
    if (this.Nodes == null) {
      this.Nodes = new();
    }
    if (this.Ways == null) {
      this.Ways = new();
    }
    var data = File.ReadAllText(this.dataPath);
    XmlDocument xmlDoc = new XmlDocument();
    xmlDoc.LoadXml(data);
    this.GetNodes(xmlDoc.SelectNodes("/osm/node"));
    this.GetWays(xmlDoc.SelectNodes("/osm/way"));
    if (this.OnFinishRead != null) {
      this.OnFinishRead.Invoke();
    }
  }

  void GetNodes(XmlNodeList xmlNodeList)
  {
    foreach (XmlNode node in xmlNodeList) {
      var osmNode = new OsmNode(node); 
      this.Nodes[osmNode.ID] = osmNode;
    }
  }

  void GetWays(XmlNodeList xmlNodeList)
  {
    foreach (XmlNode node in xmlNodeList) {
      var osmWay = new OsmWay(node);
      this.Ways.Add(osmWay);
    }
  }

  [Button("Draw ways")]
  void DrawWays()
  {
    var center = new Vector3(
      this.Bounds.Center.x,
      0,
      this.Bounds.Center.y
      );
    foreach (var way in this.Ways) {
      Color color;
      if (way.Building != null) {
        color = Color.Lerp(
          Color.yellow,
          Color.red,
          (float)way.Height / 20f
          );
      }
      else {
        color = way.IsBoundary ? Color.green: Color.blue;
      }
      for (int i = 1; i < way.Nodes.Count ; i++) {
        var p1 = way.Nodes[i - 1];
        var p2 = way.Nodes[i];
        Vector3 v1 = p1.GetPositionFrom(center);
        Vector3 v2 = p2.GetPositionFrom(center);
        Debug.DrawLine(v1, v2, color, 1.0f);
      }  
    } 
  }

  // Update is called once per frame
  void Update()
  {
    if (this.drawWays) {
      this.DrawWays();
    }
  }

  [Button("Set bounds")]
  void SetBounds()
  {
    this.Bounds = new OsmBounds(
      minLatitude: this.latitudeRange.min,
      maxLatitude: this.latitudeRange.max,
      minLongitude: this.longitudeRange.min,
      maxLongitude: this.longitudeRange.max
      );
  }
}
