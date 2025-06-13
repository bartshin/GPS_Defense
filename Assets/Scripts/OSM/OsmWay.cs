using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace OSM
{
  [Serializable]
  public class OsmWay 
  {
    const float BuildingOneLevelHeight = 3f;
    public enum HighwayType
    {
      Primary,
      Secondary,
      Tertiary
    }

    [Serializable]
    public struct WayNode
    {
      [SerializeField]
      public ulong Ref { get; private set; }
      [SerializeField]
      public float Latitude { get; private set; }
      [SerializeField]
      public float Longitude { get; private set; }
      [SerializeField]
      public float X { get; private set; }
      [SerializeField]
      public float Y { get; private set; }

      public Vector3 GetPositionFrom(Vector3 center)
      {
        return (
          new Vector3(
            this.X - center.x,
            center.y, 
            this.Y - center.z));
      }

      public WayNode(XmlNode xmlNode)
      {
        this.Ref = OsmUtility.GetAttribute<ulong>("ref", xmlNode.Attributes);
        this.Latitude = OsmUtility.GetAttribute<float>("lat", xmlNode.Attributes);
        this.Longitude = OsmUtility.GetAttribute<float>("lon", xmlNode.Attributes);
        this.X = (float)MercatorProjection.lonToX(this.Longitude);
        this.Y = (float)MercatorProjection.latToY(this.Latitude);
      }
    }
    [SerializeField]
    public string Name { get; private set;  }
    [SerializeField]
    public string Building { get; private set;  }
    [SerializeField]
    public float Height { get; private set; }
    [SerializeField]
    public ulong ID { get; private set; }
    [SerializeField]
    public OsmBounds Bounds;
    [SerializeField]
    public HighwayType Highway { get; private set; }
    [SerializeField]
    public Nullable<int> Ref { get; private set; }
    [SerializeField]
    public bool IsBoundary { get; private set; }
    public List<WayNode> Nodes;

    public Vector2 CalcCenter()
    {
      if (this.Nodes.Count > 0) {
        float totalX = 0f;
        float totalY = 0f;
        for (int i = 0; i < this.Nodes.Count; ++i) {
          totalX += this.Nodes[i].X;
          totalY += this.Nodes[i].Y;
        }
        var total = new Vector2(totalX, totalY);
        return (total / (float)this.Nodes.Count);
      }
      else {
        return (Vector2.zero);
      }
    }

    public OsmWay(XmlNode xmlNode)
    {
      this.ID = OsmUtility.GetAttribute<ulong>("id", xmlNode.Attributes);
      this.Bounds = new OsmBounds(xmlNode.SelectSingleNode("bounds"));
      this.Nodes = new ();
      var nodeList = xmlNode.SelectNodes("nd");
      foreach (XmlNode node in nodeList) {
        this.Nodes.Add(new WayNode(node)); 
      }
      if (this.Nodes.Count < 2 ||
        this.Nodes[0].Ref != this.Nodes[this.Nodes.Count - 1].Ref) {
        this.IsBoundary = false;
      }
      else {
        this.IsBoundary = true;
      }
      var tagList = xmlNode.SelectNodes("tag");
      foreach (XmlNode tag in tagList) {
        var (key, valueString) = (
          OsmUtility.GetAttribute<string>("k", tag.Attributes),
          OsmUtility.GetAttribute<string>("v", tag.Attributes)
          );
        switch (key)
        {
          case "name":
            this.Name = valueString;
            break;
          case "highway":
            if (valueString == "primary") {
              this.Highway = HighwayType.Primary;
            }
            else if (valueString == "secondary") {
              this.Highway = HighwayType.Secondary;
            }
            else {
              this.Highway = HighwayType.Tertiary;
            }
            break;
          case "ref":
            if (int.TryParse(valueString, out int refValue)) {
              this.Ref = refValue;
            }
            break;
          case "building":
            this.Building = valueString;
            break;
          case "building:levels":
             if (float.TryParse(valueString, out float buildingLevel)) {
               this.Height = BuildingOneLevelHeight * buildingLevel;
             }
            break;
          case "height":
            if (float.TryParse(valueString, out float height)) {
              this.Height = height;
            }
            break;
        }
      }
    }
  }
}
