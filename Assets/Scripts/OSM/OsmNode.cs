using System;
using System.Xml;
using UnityEngine;

namespace OSM
{
  [Serializable]
  public class OsmNode
  {
    [SerializeField]
    public string Name { get; private set; }
    [SerializeField]
    public string Amenity { get; private set; }
    [SerializeField]
    public string Shop { get; private set; }
    [SerializeField]
    public string Tourism { get; private set; }
    [SerializeField]
    public string Highway { get; private set; }
    [SerializeField]
    public int Ref { get; private set; }
    [SerializeField]
    public ulong ID { get; private set; }
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
          0, 
          this.Y - center.y)
        );
    }

    public OsmNode(XmlNode xmlNode)
    {
      this.ID = OsmUtility.GetAttribute<ulong>("id", xmlNode.Attributes);  
      this.Latitude = OsmUtility.GetAttribute<float>("lat", xmlNode.Attributes);
      this.Longitude = OsmUtility.GetAttribute<float>("lon", xmlNode.Attributes);
      this.X = (float)MercatorProjection.lonToX(this.Longitude);
      this.Y = (float)MercatorProjection.latToY(this.Latitude);
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
            this.Highway = valueString;
            break;
          case "ref":
            if (int.TryParse(valueString, out int refValue)) {
              this.Ref = refValue;
            }
            break;
          case "amenity":
            this.Amenity = valueString;
            break;
          case "shop":
            this.Shop = valueString;
            break;
          case "tourism":
            this.Tourism = valueString;
            break;
        }
      } 
    }
  }
}
