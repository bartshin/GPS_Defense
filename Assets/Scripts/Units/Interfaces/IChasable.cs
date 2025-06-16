using UnityEngine;

namespace Unit
{
  public interface IChasable : INavMeshMovable
  {
    public BaseDamagable ChaseTarget { get; set; }
  }
}
