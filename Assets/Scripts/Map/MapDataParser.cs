using System;
using System.Collections.Generic;
using Architecture;
using Road;

public class MapDataParser : Singleton<MapDataParser>
{
  public List<RoadData> ParseRoadData(JsonData jsonData)
  {
    var roads = new Dictionary<string, RoadData>();
    var data = new List<RoadData>(); 
    for (int i = 0; i < jsonData.elements.Length; i++) {
      if (jsonData.elements[i].geometry != null) {
        var parsed = this.ParseElement(jsonData.elements[i]);
        if (parsed.Name != null) {
          if (roads.TryGetValue(parsed.Name, out RoadData exist)) {
            exist.AddPath(parsed.Path, parsed.Bounds);
          }
          else {
            roads.Add(parsed.Name, parsed);
          }
        }
        data.Add(parsed);
      } 
    }
    foreach (var road in roads.Values) {
      road.Merge();
      data.Add(road);
    }
    return (data);
  }

  RoadData ParseElement(JsonData.Element element)
  {
    var bounds = new RoadBounds 
    {
      Min = new Coordinates {
        Lat = element.bounds.minlat, Lng = element.bounds.minlon 
      },
      Max = new Coordinates {
        Lat = element.bounds.maxlat, Lng = element.bounds.maxlon
      }
    };
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
    var path = new List<IdentifiableCoord>(element.geometry.Length);
    for (int i = 0; i < element.geometry.Length; i++) {
      path.Add(new IdentifiableCoord {
        Lat = element.geometry[i].lat,
        Lng = element.geometry[i].lon,
        nodeId = element.nodes[i]
        }); 
    }
    var data = new RoadData(
      name: element.tags.name,
      bounds: bounds,
      type: roadType,
      available: available,
      path: path,
      numberOfLanes: numberOfLanes
      );
    return (data);     
  }
}
