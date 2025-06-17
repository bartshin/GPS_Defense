using UnityEngine;
using UnityEngine.AI;

public interface INavMeshMovable
{
  public NavMeshAgent NavMeshAgent { get; }
  public Rigidbody Rigidbody { get; }
}
