using System.Collections.Generic;
using UnityEngine;

public interface IPatrolable : INavMeshMovable
{
  public int NextWayPoint { get; set; }
  public List<Vector3> WayPoints { get; }
}
