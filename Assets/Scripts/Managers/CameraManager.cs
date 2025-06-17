using System;
using System.Collections;
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
  [SerializeField]
  float CameraSpeed;
  [SerializeField]
  float ZoomSpeed;
  Coroutine zoomRoutine;

  protected override void Awake()
  {
    base.Awake();
  }

  public void Focus(Transform target)
  {
    this.FollowTarget.position = target.position + this.postionOffset;
    this.LookTarget.position = target.position + this.lookOffset;
  }

  public void Zoom(float zoomValue) {
    if (!this.IsZoomAble()) {
      return ;
    }
    if (this.zoomRoutine != null) {
     this.StopCoroutine(this.zoomRoutine);
    }
    this.zoomRoutine = this.StartCoroutine(this.StartZoom(zoomValue));
  }

  IEnumerator StartZoom(float zoomValue)
  {
    float zoomedValue = 0;
    float zoomAbs = Math.Abs(zoomValue);
    float step = this.ZoomSpeed;
    if (zoomValue < 0) {
      step *= -1f;
    }
    var zoomVector = new Vector3(0, step * Time.deltaTime, 0);
    while (Math.Abs(zoomedValue) < zoomAbs)
    {
      this.FollowTarget.position += zoomVector;
      this.LookTarget.position += zoomVector;
      zoomedValue += step;
      yield return (null); 
    }
  }

  public bool IsZoomAble()
  {
    if (this.LookTarget.position.y < 5f ||
      this.FollowTarget.position.y > 100f) {
      return (false);
    }
    return (true);
  }

  public void LateUpdate()
  {
    var joystickInput = GameManager.Shared.JoysticValue;
    if (joystickInput != Vector2.zero) {
      this.FollowTarget.position += new Vector3(
        joystickInput.x * this.CameraSpeed * Time.deltaTime,
        0,
        joystickInput.y * this.CameraSpeed * Time.deltaTime
        );
      this.LookTarget.position += new Vector3(
        joystickInput.x * this.CameraSpeed * Time.deltaTime,
        0,
        joystickInput.y * this.CameraSpeed * Time.deltaTime
        );
    }
  }
}
