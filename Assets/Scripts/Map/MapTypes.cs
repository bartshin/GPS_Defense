using System;
using System.Text;

namespace Road
{
  public enum Surface
  {
    Asphalt,
  } 

  [Flags]
  public enum Available
  {
    None = 0,
    Foot = 1,
    Bicycle = 2,
    Car = 3
  }

  public struct Coordinates
  {
    public float Lat;
    public float Lng;

    public override string ToString()
    {
      return ($"(lat: {this.Lat}, lng: {this.Lng})");
    }
  }

  public struct RoadBounds
  {
     public Coordinates Min;
     public Coordinates Max;

     public bool IsContain(RoadBounds bounds)
     {
       if (this.Min.Lat > bounds.Min.Lat ||
         this.Min.Lng > bounds.Min.Lng) {
         return (false);
       }
       if (this.Max.Lat < bounds.Max.Lat ||
         this.Max.Lng < bounds.Max.Lng) {
         return (false);
       }
       return (true);
     }
  }

  public struct IdentifiableCoord
  {
    public float Lat;
    public float Lng;
    public int nodeId;

    public override string ToString()
    {
      return ($"id: {this.nodeId} (lat: {this.Lat}, lng: {this.Lng})");
    }

  }

  public enum RoadType 
  {
    Normal,
    Bridge,
    Tunnel
  }

  [Serializable]
  public struct JsonData
  {
    const string YES = "yes";
    const string NO = "no";
    [Serializable]
    public struct Bounds
    {
      public float minlat;
      public float minlon;
      public float maxlat;
      public float maxlon;

      public override string ToString()
      {
        return ($"min: (lat: {this.minlat}, lng: {this.minlon}), max: (lat: {this.maxlat}, lng: {this.maxlon})");
      }
    }
    [Serializable]
    public struct Tags
    {
      public string bridge;
      public bool IsBridge => this.bridge == YES;
      public string highway;
      public string name;
      public string bicycle;
      public bool IsBicycleAvailable => this.bicycle == YES;
      public string foot;
      public bool IsFootAvailable => this.foot == YES;
      public int lanes;
      public string tunnel;
      public bool IsTunnel => this.tunnel == YES;

      public override string ToString()
      {
        var builder = new StringBuilder();
        if (this.name == null) {
          builder.AppendLine($"name: null");
        }
        else {
          builder.AppendLine($"name: {this.name.Length}");
        }
        builder.AppendLine($"highway: {this.highway}");
        builder.AppendLine($"lane: {this.lanes}");
        if (this.IsBridge) {
          builder.AppendLine("bridge");
        }
        if (this.IsTunnel) {
          builder.Append("tunnel");
        }
        if (this.IsFootAvailable) {
          builder.AppendLine("foot");
        }
        if (this.IsBicycleAvailable) {
          builder.AppendLine("bicycle");
        }
        return (builder.ToString());
      }
    }
    [Serializable]
    public struct Geometry
    {
      public float lat;
      public float lon;

      public override string ToString()
      {
        return ($"(lat: {this.lat}, lng: {this.lon})");
      }
    }
    [Serializable]
    public struct Element
    {
      public string type;
      public int id;
      public Bounds bounds;
      public Geometry[] geometry;
      public int[] nodes;
      public Tags tags;
      public override string ToString()
      {
        var builder = new StringBuilder();
        builder.AppendLine($"type: {this.type}");
        builder.AppendLine($"id: {this.id}");
        builder.AppendLine($"bounds: {this.bounds}");
        if (this.geometry != null) {
          builder.AppendLine($"geometry count: {this.geometry.Length}");
        }
        else {
          builder.AppendLine($"geometry null");
        }
        builder.AppendLine("tags");
        builder.Append(this.tags.ToString());
        return (builder.ToString());
      }
    }
    public Element[] elements;

    public override string ToString()
    {
      var builder = new StringBuilder();
      builder.AppendLine($"count: {this.elements.Length}");
      for (int i = 0; i < this.elements.Length; i++) {
        builder.Append($"index: {i}\n{this.elements[i]}");
      }
      return (builder.ToString());
    }

  }
}
