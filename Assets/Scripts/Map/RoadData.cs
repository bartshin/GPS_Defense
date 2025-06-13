using System;
using System.Text;
using System.Collections.Generic;
using Road;

public class RoadData 
{
  const float MERGE_DIST_THRESHOLD = 0.001f;
  public string Name { get; private set; }
  public RoadBounds Bounds { get; private set; }
  public RoadType Type { get; private set; }
  public int NumberOfLanes { get; private set; }
  public Available RoadAvailable { get; private set; }
  public List<List<IdentifiableCoord>> AllRoad 
  {
    get {
      if (this.afterMerged != null) {
        return (this.afterMerged);
      }
      this.afterMerged = (new List<List<IdentifiableCoord>>() { this.Path });
      return (this.afterMerged);
    }
  }
  List<List<IdentifiableCoord>> afterMerged;
  public List<IdentifiableCoord> Path { get; private set; }
  List<(List<IdentifiableCoord> path, RoadBounds bounds)> allPath;

  public RoadData(
    in string name,
    RoadBounds bounds,
    RoadType type,
    Available available,
    List<IdentifiableCoord> path,
    int numberOfLanes = 1
    )
  {
    this.Name = name;
    this.Bounds = bounds;
    this.Type = type;
    this.RoadAvailable = available;
    this.NumberOfLanes = numberOfLanes;
    this.Path = path;
    this.allPath = new ();
  }

  public void Merge()
  {
    if (this.allPath.Count == 0) {
      return ;
    }
    HashSet<int> merged = new();
    int tryCount = 0;
    int maxTry = this.allPath.Count * this.allPath.Count;
    while (merged.Count < this.allPath.Count - 1 && tryCount++ < maxTry) {
      for (int i = 0; i < this.allPath.Count; ++i) {
        if (merged.Contains(i)) {
          break;
        }
        var path = this.allPath[i].path;
        var currentStart = this.Path[0];
        var currentEnd = this.Path[this.Path.Count - 1];
        var pathStart = path[0];
        var pathEnd = path[path.Count - 1];
        var currentBounds = this.Bounds;
        var pathBounds = this.allPath[i].bounds;
        if (currentBounds.IsContain(pathBounds)) {
          merged.Add(i);
        }
        else if (pathBounds.IsContain(currentBounds)) {
          this.Path = path;
          this.Bounds = pathBounds;
          merged.Add(i);
        }
        else if (pathStart.nodeId == currentEnd.nodeId) {
          this.Path.AddRange(path);
          merged.Add(i);
        }
        else if (pathEnd.nodeId == currentStart.nodeId) {
          path.AddRange(this.Path);
          this.Path = path;
          merged.Add(i);
        }
      }
    }
    this.afterMerged = new();
    this.afterMerged.Add(this.Path);
    for (int i = 0; i < this.allPath.Count; i++) {
      if (!merged.Contains(i)) {
        this.afterMerged.Add(this.allPath[i].path);
      }
    }
  }
  
  public void AddPath(List<IdentifiableCoord> path, RoadBounds bounds)
  {
    this.allPath.Add((path, bounds));
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
