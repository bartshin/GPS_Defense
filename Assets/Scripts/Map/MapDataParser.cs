using System;
using System.Collections.Generic;
using UnityEngine;
using Architecture;
using Road;

public class MapDataParser : Singleton<MapDataParser>
{
  public List<RoadData> ParseRoadData(JsonData jsonData)
  {
    var data = new List<RoadData>(jsonData.elements.Length); 
    for (int i = 0; i < jsonData.elements.Length; i++) {
      if (jsonData.elements[i].geometry != null) {
        data.Add(this.ParseElement(jsonData.elements[i]));
      } 
    }
    return (data);
  }

  RoadData ParseElement(JsonData.Element element)
  {
    (Coordinates, Coordinates) bounds = (
      new Coordinates {
      Lat = element.bounds.minlat, Lng = element.bounds.minlon 
      },
      new Coordinates {
      Lat = element.bounds.maxlat, Lng = element.bounds.maxlon
      } 
      );
    RoadType roadType = RoadType.Normal;
    if (element.tags.IsBridge) {
      roadType = RoadType.Bridge;
    }
    else if (element.tags.IsTunnel) {
      roadType = RoadType.Tunnel;
    }
    var numberOfLanes = Math.Max(element.tags.lanes, 1);
    var available = Available.None;
    if (element.tags.IsFootAvailable) {
      available |= Available.Foot;
    }
    if (element.tags.IsBicycleAvailable) {
      available |= Available.Bicycle;
    }
    if (available == Available.None) {
      available = Available.Car;
    }

    var data = new RoadData(
      name: element.tags.name,
      bounds: bounds,
      type: roadType,
      available: available,
      numberOfLanes: numberOfLanes,
      pathCount: element.geometry.Length
      );
    for (int i = 0; i < element.geometry.Length; i++) {
      data.AddPath(new Coordinates {
        Lat = element.geometry[i].lat,
        Lng = element.geometry[i].lon
        }); 
    }
    return (data);     
  }
}
