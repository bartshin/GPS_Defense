using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using Road;

public struct RoadData 
{
  public string Name { get; private set; }
  public (Coordinates min, Coordinates max) Bounds { get; private set; }
  public RoadType Type { get; private set; }
  public int NumberOfLanes { get; private set; }
  public Available RoadAvailable { get; private set; }
  public List<Coordinates> Path { get; private set; }

  public RoadData(
    in string name,
    (Coordinates min, Coordinates max) bounds,
    RoadType type,
    Available available,
    int numberOfLanes = 1,
    int pathCount = 0
    )
  {
    this.Name = name;
    this.Bounds = bounds;
    this.Type = type;
    this.RoadAvailable = available;
    this.NumberOfLanes = numberOfLanes;
    this.Path = pathCount == 0 ? new List<Coordinates>(): new List<Coordinates>(pathCount);
  }

  public void AddPath(Coordinates coord)
  {
    this.Path.Add(coord);
  }

  public void AddPath(ICollection<Coordinates> pathArray)
  {
    foreach (var coord in pathArray) {
      this.AddPath(coord); 
    }  
  }

  public override string ToString()
  {
    var builder = new StringBuilder();
    builder.AppendLine($"Name: {this.Name}");
    builder.AppendLine($"Type: {this.Type}");
    builder.AppendLine($"Available: {this.RoadAvailable}");
    builder.AppendLine($"Lanes: {this.NumberOfLanes}");
    builder.AppendLine($"Path: {this.Path.Count}");
    return (builder.ToString());
  }
}
