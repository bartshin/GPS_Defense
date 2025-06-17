using UnityEngine;
using Architecture;

public class CameraManager : SingletonBehaviour<CameraManager>
{
  [SerializeField]
  Transform LookTarget;
  [SerializeField]
  Transform FollowTarget;
  [SerializeField]
  Vector3 postionOffset;
  [SerializeField]
  Vector3 lookOffset;

  protected override void Awake()
  {
    base.Awake();
  }

  public void Focus(Transform target)
  {
    this.FollowTarget.position = target.position + this.postionOffset;
    this.LookTarget.position = target.position + this.lookOffset;
  }
}
