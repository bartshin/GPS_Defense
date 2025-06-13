using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using UnityEngine;
using OSM;
using Sirenix.OdinInspector;

public class MapReader : _MonoBehaviour
{
  [SerializeField] [FilePath(AbsolutePath = true, ParentFolder = "Assets/Data", Extensions = "xml")]
  string dataPath;
  [ShowInInspector] 
  Dictionary<ulong, OsmNode> nodes = new ();
  [ShowInInspector] 
  List<OsmWay> ways = new ();
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
  OsmBounds bounds;
  [SerializeField]
  bool drawWays;
  
  [Button("Clear data")]
  void ClearData()
  {
    this.nodes.Clear();
    this.ways.Clear();
  }

  [Button("Read Data File")]
  void ReadDataFile()
  {
    var data = File.ReadAllText(this.dataPath);
    XmlDocument xmlDoc = new XmlDocument();
    xmlDoc.LoadXml(data);
    this.GetNodes(xmlDoc.SelectNodes("/osm/node"));
    this.GetWays(xmlDoc.SelectNodes("/osm/way"));
  }

  void GetNodes(XmlNodeList xmlNodeList)
  {
    foreach (XmlNode node in xmlNodeList) {
      var osmNode = new OsmNode(node); 
      this.nodes[osmNode.ID] = osmNode;
    }
  }

  void GetWays(XmlNodeList xmlNodeList)
  {
    foreach (XmlNode node in xmlNodeList) {
      var osmWay = new OsmWay(node);
      this.ways.Add(osmWay);
    }
  }

  [Button("Draw ways")]
  void DrawWays()
  {
    foreach (var way in this.ways) {
      for (int i = 1; i < way.Nodes.Count ; i++) {
        var p1 = way.Nodes[i - 1];
        var p2 = way.Nodes[i];
        Vector3 v1 = this.GetPosition(p1);
        Vector3 v2 = this.GetPosition(p2);
        Debug.DrawLine(v1, v2, Color.blue, 1.0f);
      }  
    } 
  }

  Vector3 GetPosition(OsmNode node)
  {
    return (
      new Vector3(
        node.X - this.bounds.Center.x,
        0, 
        node.Y - this.bounds.Center.y));
  }

  Vector3 GetPosition(OsmWay.WayNode node)
  {
    return (
      new Vector3(
        node.X - this.bounds.Center.x,
        0, 
        node.Y - this.bounds.Center.y));
  }

  // Update is called once per frame
  void Update()
  {
    if (this.drawWays) {
      this.DrawWays();
    }
  }

  [Button("Set Bounds")]
  void SetBounds()
  {
    this.bounds = new OsmBounds(
      minLatitude: this.latitudeRange.min,
      maxLatitude: this.latitudeRange.max,
      minLongitude: this.longitudeRange.min,
      maxLongitude: this.longitudeRange.max
      );
  }
}
