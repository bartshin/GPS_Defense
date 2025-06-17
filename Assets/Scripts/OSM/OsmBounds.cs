using System;
using System.Xml;
using UnityEngine;

namespace OSM
{
  [Serializable]
  public struct OsmBounds
  {
    [SerializeField]
    public float MinLatitude { get; private set; }
    [SerializeField]
    public float MaxLatitude { get; private set; }
    [SerializeField]
    public float MinLongitude { get; private set; }
    [SerializeField]
    public float MaxLongitude { get; private set; }
    [SerializeField]
    public Vector2 Center { get; private set; }
    
    public Vector2 CalcRange() 
    {
      Vector2 xRange = new Vector2(
        (float)(MercatorProjection.lonToX(this.MinLongitude)),
        (float)(MercatorProjection.lonToX(this.MaxLongitude))
        );
      Vector2 yRange = new Vector2(
        (float)(MercatorProjection.latToY(this.MinLatitude)),
        (float)(MercatorProjection.latToY(this.MaxLatitude))
        );
      return (new Vector2(
          xRange.y - xRange.x,
          yRange.y - yRange.x
          ));
    }

    public OsmBounds(
      float minLatitude,
      float maxLatitude,
      float minLongitude,
      float maxLongitude
      )
    {
      this.MinLatitude = minLatitude;
      this.MaxLatitude = maxLatitude;
      this.MinLongitude = minLongitude;
      this.MaxLongitude = maxLongitude;
      this.Center = (new Vector2(
          (float)((MercatorProjection.lonToX(this.MinLongitude) +
          MercatorProjection.lonToX(this.MaxLongitude)) * 0.5),
          (float)((MercatorProjection.latToY( this.MinLatitude) +
            MercatorProjection.latToY(this.MaxLatitude)) * 0.5))
        );
    }

    public OsmBounds(XmlNode xmlNode)
    {
      this.MinLatitude = OsmUtility.GetAttribute<float>("minlat", xmlNode.Attributes);
      this.MaxLatitude = OsmUtility.GetAttribute<float>("maxlat", xmlNode.Attributes);
      this.MinLongitude = OsmUtility.GetAttribute<float>("minlon", xmlNode.Attributes);
      this.MaxLongitude = OsmUtility.GetAttribute<float>("maxlon", xmlNode.Attributes);
      this.Center = (new Vector2(
          (float)((MercatorProjection.lonToX(this.MinLongitude) +
          MercatorProjection.lonToX(this.MaxLongitude)) * 0.5),
          (float)((MercatorProjection.latToY( this.MinLatitude) +
            MercatorProjection.latToY(this.MaxLatitude)) * 0.5))
        );
    }
  }

}
