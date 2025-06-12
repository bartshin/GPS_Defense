using System;
using System.Collections.Generic;
using UnityEngine;
using Architecture;
using Road;

public class GPSConverter: Singleton<GPSConverter>
{
  public Coordinates Center { get; private set; }
  public float ZoomLevel { get; private set; }

  public void Init(Coordinates center, float zoomLevel)
  {
    this.Center = center;
    this.ZoomLevel = zoomLevel;
  }
  
  public Vector2[] Convert(IList<Coordinates> coordinates)
  {
    var positions = new Vector2[coordinates.Count];
    for (int i = 0; i < positions.Length; i++) {
      positions[i] = this.Convert(coordinates[i]); 
    }
    return (positions);
  }

  public Vector2 Convert(Coordinates coordinate)
  {
    return (
      new Vector2(
        (coordinate.Lat - this.Center.Lat) * this.ZoomLevel,
        (coordinate.Lng - this.Center.Lng) * this.ZoomLevel
        )
      ); 
  }
}
